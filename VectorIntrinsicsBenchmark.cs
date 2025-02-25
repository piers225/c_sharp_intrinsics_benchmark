using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

public static unsafe class CompareHelper
{
    public static bool NotEqualManual(int[] x, int[] y)
    {
        for (int i = 0; i < x.Length; i++)
        {
            if(x[i] == y[i])
            {
                return false;
            }
        }
        return true;
    }

    public static bool NotEqualVector(int[] x, int[] y)
    {
        int vectorSize = Vector<int>.Count; 
        int vectorLength = x.Length / vectorSize * vectorSize;   

        for (int i = 0; i < vectorLength; i += vectorSize)
        {
            var vectorX = new Vector<int>(x, i); 
            var vectorY = new Vector<int>(y, i); 
            Vector<int> result = Vector.Equals(vectorX, vectorY);
            if (!Vector<int>.Zero.Equals(result))
            {
                return false;
            }
        }
        return true;
    }

    public static bool NotEqualSse42(ref int[] x, ref int[] y)
    {
        fixed(int* xp = &x[0])
        fixed(int* yp = &y[0])
        {
            for(int i = 0; i < x.Length; i += 4)
            {   
                Vector128<int> xVector = Sse2.LoadVector128(xp + i);
                Vector128<int> yVector = Sse2.LoadVector128(yp + i);
                Vector128<int> mask = Sse2.CompareEqual(xVector, yVector);
                if (!Sse42.TestZ(mask, mask))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool NotEqualAvx2(ref int[] x, ref int[] y)
    {
        fixed(int* xp = &x[0])
        fixed(int* yp = &y[0])
        {
            for(int i = 0; i < x.Length; i += 8)
            {
                Vector256<int> xVector = Avx2.LoadVector256(xp + i);
                Vector256<int> yVector = Avx2.LoadVector256(yp + i);
                Vector256<int> mask = Avx2.CompareEqual(xVector, yVector);
                if (!Avx2.TestZ(mask, mask))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

[DisassemblyDiagnoser]
public class VectorIntrinsicsBenchmark
{
    private const int n = 100_000;
    private int[] x = new int[n];
    private int[] y = new int[n];

    [GlobalSetup]
    public void Setup()
    {
        Array.Fill(x, 1);
        Array.Fill(y, 2);
    }

    [Benchmark(Baseline = true)]
    public bool Manual() => CompareHelper.NotEqualManual(x, y);

    [Benchmark()]
    public bool Vector() => CompareHelper.NotEqualVector(x, y);

    [Benchmark()]
    public bool Sse42() => CompareHelper.NotEqualSse42(ref x, ref y);

    [Benchmark()]
    public bool Avx2() => CompareHelper.NotEqualAvx2(ref x, ref y);

}