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

    [Benchmark]
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
    public int LinqSum()
    {
        return array.Sum();
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

    [Benchmark]
    public int ManualMin()
    {
        int min = int.MaxValue;
        for(var i = 0; i < array.Length; i++)
        {
            min = array[i] < min ? array[i] : min;
        }
        return min;
    }

    [Benchmark]
    public int LinqMin()
    {
        return array.Min();
    }

    [Benchmark]
    public int SIMDMin()
    {
        int vectorSize = Vector<int>.Count; 

        int vectorLength = array.Length / vectorSize * vectorSize;  
        Vector<int> minVector = new Vector<int>(0); 

        for (int i = 0; i < vectorLength; i += vectorSize)
        {
            minVector = Vector.Min(minVector, new Vector<int>(array, i)); 
        }
        
        return Math.Min(Math.Min(Math.Min(minVector[0], minVector[1]), minVector[2]), minVector[3]);
    }
}