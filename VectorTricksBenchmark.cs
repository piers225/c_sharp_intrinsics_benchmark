using System.Numerics;
using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser]
public class VectorTricksBenchmark
{
    int[] intArray;
    short[] shortArray;
    decimal[] decimalArray;

    [GlobalSetup]
    public void Setup()
    {
        intArray = new int[100_000];
        Array.Fill(intArray, 2);

        shortArray = new short[100_000];
        Array.Fill<short>(shortArray, 2);

        decimalArray = new decimal[100_000];
        Array.Fill(decimalArray, 2);
    }

    [Benchmark]
    public int ManualSum()
    {
        int total = 0;
        for(var i = 0; i < intArray.Length; i++)
        {
            total += intArray[i];
        }
        return total;
    }

    [Benchmark]
    public int LinqSum()
    {
        return intArray.Sum();
    }

    [Benchmark]
    public int SIMDSum()
    {
        int vectorSize = Vector<int>.Count; 

        int vectorLength = intArray.Length / vectorSize * vectorSize;  
        Vector<int> sumVector = new Vector<int>(0); 

        for (int i = 0; i < vectorLength; i += vectorSize)
        {
            sumVector += new Vector<int>(intArray, i); 
        }
        
        return Vector.Sum(sumVector);
    }

    [Benchmark]
    public int ManualMin()
    {
        int min = int.MaxValue;
        for(var i = 0; i < intArray.Length; i++)
        {
            min = intArray[i] < min ? intArray[i] : min;
        }
        return min;
    }

    [Benchmark]
    public int SIMDMin()
    {
        int vectorSize = Vector<int>.Count; 

        int vectorLength = intArray.Length / vectorSize * vectorSize;  
        Vector<int> minVector = new Vector<int>(0); 

        for (int i = 0; i < vectorLength; i += vectorSize)
        {
            minVector = Vector.Min(minVector, new Vector<int>(intArray, i)); 
        }
        
        return Math.Min(Math.Min(Math.Min(minVector[0], minVector[1]), minVector[2]), minVector[3]);
    }

    [Benchmark]
    public int LinqIntMin()
    {
        return intArray.Min();
    }

    [Benchmark]
    public int LinqShortMin()
    {
        return shortArray.Min();
    }

    [Benchmark]
    public decimal LinqDecimalMin()
    {
        return decimalArray.Min();
    }
}