using System.Numerics;
using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser]
public class VectorOperationsBenchmark
{
    public struct MyVector
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public MyVector(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    private MyVector v1_Manual;
    private MyVector v2_Manual;
    private MyVector v3_Manual;
    private MyVector v4_Manual;
    private Vector<double> v1_SIMD;
    private Vector<double> v2_SIMD;
    private Vector<double> v3_SIMD;
    private Vector<double> v4_SIMD;
    private static readonly Random random = new Random();

    [GlobalSetup]
    public void Setup()
    {
        v1_Manual = new MyVector(random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0);
        v2_Manual = new MyVector(random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0);
        v3_Manual = new MyVector(random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0);
        v4_Manual = new MyVector(random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0);

        v1_SIMD = new Vector<double>(new double[] { random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0 });
        v2_SIMD = new Vector<double>(new double[] { random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0 });
        v3_SIMD = new Vector<double>(new double[] { random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0 });
        v4_SIMD = new Vector<double>(new double[] { random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0 });
    }

    [Benchmark]
    public MyVector ManualArithmetic()
    {
        return new MyVector((v1_Manual.X + v2_Manual.X) * v3_Manual.X / v4_Manual.X, 
                        (v1_Manual.Y + v2_Manual.Y) * v3_Manual.Y / v4_Manual.Y, 
                        (v1_Manual.Z + v2_Manual.Z) * v3_Manual.Z / v4_Manual.Z);
    }

    [Benchmark]
    public Vector<double> SIMDArithmetic()
    {
        return (v1_SIMD + v2_SIMD) * v3_SIMD / v4_SIMD;
    }

    [Benchmark]
    public double ManualDotProduct()
    {
        return v1_Manual.X * v2_Manual.X + v1_Manual.Y * v2_Manual.Y + v1_Manual.Z * v2_Manual.Z +
          v3_Manual.X * v4_Manual.X + v3_Manual.Y * v4_Manual.Y + v3_Manual.Z * v4_Manual.Z;
    }

    [Benchmark]
    public double SIMDDotProduct()
    {
        return Vector.Dot(v1_SIMD, v2_SIMD) + Vector.Dot(v3_SIMD, v4_SIMD);
    }

    [Benchmark]
    public MyVector ManualAbs()
    {
        return new MyVector(
            Math.Abs(v1_Manual.X) + Math.Abs(v2_Manual.X) + Math.Abs(v3_Manual.X) + Math.Abs(v4_Manual.X),
            Math.Abs(v1_Manual.Y) + Math.Abs(v2_Manual.Y) + Math.Abs(v3_Manual.Y) + Math.Abs(v4_Manual.Y),
            Math.Abs(v1_Manual.Z) + Math.Abs(v2_Manual.Z) + Math.Abs(v3_Manual.Z) + Math.Abs(v4_Manual.Z)
        );
    }

    [Benchmark]
    public Vector<double> SIMDAbs()
    {
        return Vector.Abs(v1_SIMD) + Vector.Abs(v2_SIMD) + Vector.Abs(v3_SIMD) + Vector.Abs(v4_SIMD);
    }

    [Benchmark]
    public MyVector ManualMin()
    {
        return new MyVector(
            Math.Min(Math.Min(Math.Min(v1_Manual.X, v2_Manual.X), v3_Manual.X), v4_Manual.X),
            Math.Min(Math.Min(Math.Min(v1_Manual.Y, v2_Manual.Y), v3_Manual.Y), v4_Manual.Y),
            Math.Min(Math.Min(Math.Min(v1_Manual.Z, v2_Manual.Z), v3_Manual.Z), v4_Manual.Z)
        );
    }

    [Benchmark]
    public Vector<double> SIMDMin()
    {
        return Vector.Min(Vector.Min(Vector.Min(v1_SIMD, v2_SIMD), v3_SIMD), v4_SIMD);
    }

    [Benchmark]
    public double ManualSumAll()
    {
        return v1_Manual.X + v1_Manual.Y + v1_Manual.Z + 
                v2_Manual.X + v2_Manual.Y + v2_Manual.Z +
                v3_Manual.X + v3_Manual.Y + v3_Manual.Z +
                v4_Manual.X + v4_Manual.Y + v4_Manual.Z;
    }

    [Benchmark]
    public double SIMDSumAll()
    {
        return Vector.Sum(v1_SIMD) + Vector.Sum(v2_SIMD) + Vector.Sum(v3_SIMD) + Vector.Sum(v4_SIMD);
    }

    [Benchmark]
    public bool ManualLessThanOrEqualAll()
    {
        return (v1_Manual.X <= v2_Manual.X && 
                v1_Manual.Y <= v2_Manual.Y && 
                v1_Manual.Z <= v2_Manual.Z) ||
                (v3_Manual.X >= v4_Manual.X && 
                v3_Manual.Y >= v4_Manual.Y && 
                v3_Manual.Z >= v4_Manual.Z); 
    }

    [Benchmark]
    public bool SIMDLessThanOrEqualAll()
    {
        return Vector.LessThanOrEqualAll(v1_SIMD, v2_SIMD) || Vector.GreaterThanOrEqualAll(v3_SIMD, v4_SIMD);
    }
}