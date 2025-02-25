using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser]
public class VectorPrimesBenchmark
{
    private const int MAX_NUMBER = 10_000_000;

    [Benchmark]
    public void IsPrimeManual()
    {
        for (int i = 1; i <= MAX_NUMBER; i++)
        {
            IsPrime(i);
        }
    }

    [Benchmark]
    public void IsPrimeVectorSIMD()
    {
        float[] array = new float[Vector<float>.Count];
        array[0] = 0;
        array[1] = 2;
        array[2] = 6;
        array[3] = 8;
        var offset = new Vector<float>(array);
        int increment = Vector<float>.Count * 3;
        for (int i = 1; i <= MAX_NUMBER; i++)
        {
            IsPrimeVectorSIMD(i, increment, ref offset);
        }
    }

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

    public bool IsPrimeVectorSIMD(int n, int increment, ref Vector<float> offset)
    {
        if (n <= 1)
            return false;
        if (n == 2 || n == 3)
            return true;
        if (n % 2 == 0 || n % 3 == 0)
            return false;

        int sqrtN = (int)Math.Sqrt(n);
        var original = new Vector<float>(n);
        for (int i = 5; i <= sqrtN; i += increment)
        {
            var baseVector = Vector<float>.One * i;
            var divisor = offset + baseVector;
            var result = original / divisor;
            if (Vector.EqualsAny(result, Vector.Floor(result)))
            {
                return false;
            }
        }

        return true;
    }
}