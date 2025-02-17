using System.Numerics;
using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser]
public class VectorTricksBenchmark
{
    int[] array;

    [GlobalSetup]
    public void Setup()
    {
        array = new int[100_000];
        Array.Fill(array, 2);
    }

    [Benchmark(Baseline = true)]
    public int ManualSum()
    {
        int total = 0;
        for(var i = 0; i < array.Length; i++)
        {
            total += array[i];
        }
        return total;
    }

    [Benchmark]
    public int SIMDSum()
    {
        int vectorSize = Vector<int>.Count; 

        int vectorLength = array.Length / vectorSize * vectorSize;  
        Vector<int> sumVector = new Vector<int>(0); 

        for (int i = 0; i < vectorLength; i += vectorSize)
        {
            sumVector += new Vector<int>(array, i); 
        }
        
        return Vector.Sum(sumVector);
    }
}