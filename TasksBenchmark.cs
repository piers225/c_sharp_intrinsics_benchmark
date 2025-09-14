using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Threading.Tasks.Sources;
using System.Threading.Tasks.Dataflow;
using System.Linq;
using System.Threading.Channels;
using System.Collections.Concurrent;

[MemoryDiagnoser()]
[WarmupCount(2)]
[IterationCount(4)]
public class TasksBenchmark
{
    // Change these Params for your runs.
    // WARNING: large MaxValue (e.g., 1_000_000_000) will take a very long time.
    [Params(10_000_000, 250_000_000 /*, 1_000_000_000 */)]
    public int MaxValue;

    // Batch sizes: partitions of the range [1..MaxValue]
    [Params(100_000, 1_000_000)]
    public int BatchSize;

    // -1 => no limit for Task.Run/Parallel.For (let each decide).
    // 0 => use Environment.ProcessorCount
    // >0 => specific MaxDegreeOfParallelism
    [Params(-1 /*, 0 */)]
    public int MaxDegreeOfParallelism;

    private int batchCount;

    [GlobalSetup]
    public void Setup()
    {
        batchCount = (MaxValue + BatchSize - 1) / BatchSize;
    }

    private int ResolveMaxDegree()
    {
        if (MaxDegreeOfParallelism == -1) return -1;
        if (MaxDegreeOfParallelism == 0) return Environment.ProcessorCount;
        return MaxDegreeOfParallelism;
    }

    // 1) Task.Run with no explicit concurrency limit
    [Benchmark(Description = "Task.Run (no limit)")]
    public async Task<long> TaskRun_NoLimit()
    {
        var tasks = new List<Task<int>>(batchCount);
        for (int i = 0; i < batchCount; i++)
        {
            int idx = i;
            tasks.Add(Task.Run(() => ProcessBatch(idx)));
        }

        var results = await Task.WhenAll(tasks);
        return results.Sum();
    }

    // 2) Task.Run but limited with SemaphoreSlim to match MaxDegreeOfParallelism
    [Benchmark(Description = "Task.Run + SemaphoreSlim (limited)")]
    public async Task<long> TaskRun_Semaphore()
    {
        int md = ResolveMaxDegree();
        // if md == -1 (unbounded), behave like TaskRun_NoLimit
        if (md == -1) return await TaskRun_NoLimit();

        using var sem = new SemaphoreSlim(md, md);
        var tasks = new List<Task<int>>(batchCount);
        for (int i = 0; i < batchCount; i++)
        {
            int idx = i;
            tasks.Add(Task.Run(() =>
            {
                sem.Wait();
                try
                {
                    return ProcessBatch(idx);
                }
                finally
                {
                    sem.Release();
                }
            }));
        }

        var results = await Task.WhenAll(tasks);
        return results.Sum();
    }

    // 3) Parallel.For (with ParallelOptions)
    [Benchmark(Description = "Parallel.For")]
    public long ParallelFor_PerBatch()
    {
        int md = ResolveMaxDegree();

        long total = 0;
        var po = new ParallelOptions();
        if (md > 0) po.MaxDegreeOfParallelism = md;
        // if md == -1 leave default

        Parallel.For(0, batchCount,
            po,
            () => 0L,
            (i, state, local) =>
            {
                local += ProcessBatch(i);
                return local;
            },
            local => Interlocked.Add(ref total, local)
        );

        return total;
    }

    // 4) Parallel.ForEach over Enumerable.Range
    [Benchmark(Description = "Parallel.ForEach")]
    public long ParallelForEach_PerBatch()
    {
        int md = ResolveMaxDegree();
        long total = 0;
        var po = new ParallelOptions();
        if (md > 0) po.MaxDegreeOfParallelism = md;

        Parallel.ForEach(Enumerable.Range(0, batchCount), po, () => 0L, (i, state, local) =>
        {
            local += ProcessBatch(i);
            return local;
        },
        local => Interlocked.Add(ref total, local));

        return total;
    }

    // 5) PLINQ
    [Benchmark(Description = "PLINQ AsParallel()")]
    public long PLINQ_PerBatch()
    {
        int md = ResolveMaxDegree();
        var query = Enumerable.Range(0, batchCount).AsParallel();
        if (md > 0) query = query.WithDegreeOfParallelism(md);

        // Use Select(ProcessBatch) then Sum
        return query.Select(i => ProcessBatch(i)).Sum(i => (long)i);
    }

    // 6) TPL Dataflow ActionBlock
    [Benchmark(Description = "TPL Dataflow ActionBlock")]
    public async Task<long> Dataflow_ActionBlock_PerBatch()
    {
        int md = ResolveMaxDegree();
        var options = new ExecutionDataflowBlockOptions();
        if (md > 0) options.MaxDegreeOfParallelism = md;
        // if md == -1, leave default (unbounded) which Dataflow treats as 1 by default;
        // to treat -1 as Environment.ProcessorCount for Dataflow, we map it:
        if (md == -1) options.MaxDegreeOfParallelism = Environment.ProcessorCount;

        long total = 0;
        var block = new ActionBlock<int>(i =>
        {
            Interlocked.Add(ref total, ProcessBatch(i));
        }, options);

        for (int i = 0; i < batchCount; i++) block.Post(i);
        block.Complete();
        await block.Completion;
        return total;
    }

    // 7) Channel worker pool where workers directly Interlocked.Add to total
    [Benchmark(Description = "Channel workers (workers add to total)")]
    public async Task<long> Channel_Workers_AddDirect_PerBatch()
    {
        int md = ResolveMaxDegree();
        if (md == -1) md = Environment.ProcessorCount;

        var indexChannel = Channel.CreateUnbounded<int>(new UnboundedChannelOptions { SingleWriter = true, SingleReader = false });
        var writer = indexChannel.Writer;
        for (int i = 0; i < batchCount; i++) writer.TryWrite(i);
        writer.Complete();

        long total = 0L;
        var workers = new Task[md];
        for (int w = 0; w < md; w++)
        {
            workers[w] = Task.Run(async () =>
            {
                var reader = indexChannel.Reader;
                await foreach (var idx in reader.ReadAllAsync().ConfigureAwait(false))
                {
                    Interlocked.Add(ref total, ProcessBatch(idx));
                }
            });
        }

        await Task.WhenAll(workers);
        return total;
    }

    // 8) Channel worker pool where workers write results to a results channel,
    //    and a single reader aggregates the results (your requested pattern)
    [Benchmark(Description = "Channel workers -> results channel -> single reader")]
    public async Task<long> Channel_Workers_WriteResults_PerBatch()
    {
        int md = ResolveMaxDegree();
        if (md == -1) md = Environment.ProcessorCount;

        var q = new ConcurrentQueue<int>(Enumerable.Range(0, batchCount));

        // Channel of results (each worker writes its per-batch count)
        var resultChannel = Channel.CreateUnbounded<int>(new UnboundedChannelOptions { SingleWriter = false, SingleReader = true });
        var resultWriter = resultChannel.Writer;
        var resultReader = resultChannel.Reader;

        // Start worker tasks that read indices and write counts to resultChannel
        var workers = new Task[md];
        for (int w = 0; w < md; w++)
        {
            workers[w] = Task.Run(async () =>
            {
                while (q.TryDequeue(out var idx))
                {
                    int count = ProcessBatch(idx);
                    // Write result for aggregation
                    await resultWriter.WriteAsync(count).ConfigureAwait(false);
                }
            });
        }

        // Reader task: single reader that aggregates results
        long total = 0L;
        var readerTask = Task.Run(async () =>
        {
            await foreach (var v in resultReader.ReadAllAsync().ConfigureAwait(false))
            {
                total += v;
            }
        });

        // Wait for workers to finish producing results, then complete the result writer
        await Task.WhenAll(workers);
        resultChannel.Writer.Complete();
        // Wait for reader to finish consuming
        await readerTask;
        return total;
    }

    // 9) Parallel.ForAsync (async parallel API)
    [Benchmark(Description = "Parallel.ForAsync")]
    public async Task<long> Parallel_ForAsync_PerBatch()
    {
        int md = ResolveMaxDegree();
        var po = new ParallelOptions();
        if (md > 0) po.MaxDegreeOfParallelism = md;

        long total = 0L;
        // Parallel.ForAsync returns a ValueTask; wait synchronously for benchmark parity
        await Parallel.ForAsync(0, batchCount, po, (i, ct) =>
        {
            Interlocked.Add(ref total, ProcessBatch(i));
            return ValueTask.CompletedTask;
        });

        return total;
    }

    // 10) Parallel.ForEachAsync (async parallel API over an enumerable)
    [Benchmark(Description = "Parallel.ForEachAsync")]
    public async Task<long> Parallel_ForEachAsync_PerBatch()
    {
        int md = ResolveMaxDegree();
        var po = new ParallelOptions();
        if (md > 0) po.MaxDegreeOfParallelism = md;

        long total = 0L;
        var items = Enumerable.Range(0, batchCount);
        await Parallel.ForEachAsync(items, po, (i, ct) =>
        {
            Interlocked.Add(ref total, ProcessBatch(i));
            return ValueTask.CompletedTask;
        });

        return total;
    }


    // Process a single batch by calling the provided IsPrime on every integer in the batch
    private int ProcessBatch(int batchIndex)
    {
        int start = batchIndex * BatchSize + 1;
        int end = Math.Min(start + BatchSize - 1, MaxValue);
        int count = 0;
        for (int n = start; n <= end; n++)
        {
            if (IsPrime(n)) count++;
        }
        return count;
    }

    // The IsPrime method you provided
    public bool IsPrime(int n)
    {
        if (n <= 1)
            return false;
        if (n == 2 || n == 3)
            return true;
        if (n % 2 == 0 || n % 3 == 0)
            return false;

        int sqrtN = (int)Math.Sqrt(n);
        for (int i = 5; i <= sqrtN; i += 6)
        {
            if (n % i == 0 || n % (i + 2) == 0)
                return false;
        }

        return true;
    }
}