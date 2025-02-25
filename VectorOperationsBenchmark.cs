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

    private MyVector v1_Original;
    private MyVector v2_Original;
    private MyVector v3_Original;
    private MyVector v4_Original;
    private Vector<double> v1_New;
    private Vector<double> v2_New;
    private Vector<double> v3_New;
    private Vector<double> v4_New;
    private static readonly Random random = new Random();

    [GlobalSetup]
    public void Setup()
    {
        v1_Original = new MyVector(random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0);
        v2_Original = new MyVector(random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0);
        v3_Original = new MyVector(random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0);
        v4_Original = new MyVector(random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0);

        v1_New = new Vector<double>(new double[] { random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0 });
        v2_New = new Vector<double>(new double[] { random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0 });
        v3_New = new Vector<double>(new double[] { random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0 });
        v4_New = new Vector<double>(new double[] { random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0, random.NextDouble() * 10.0 });
    }

    [Benchmark]
    public MyVector ManualArithmetic()
    {
        return new MyVector((v1_Original.X + v2_Original.X) * v3_Original.X / v4_Original.X, 
                        (v1_Original.Y + v2_Original.Y) * v3_Original.Y / v4_Original.Y, 
                        (v1_Original.Z + v2_Original.Z) * v3_Original.Z / v4_Original.Z);
    }

    [Benchmark]
    public Vector<double> SIMDArithmetic()
    {
        return (v1_New + v2_New) * v3_New / v4_New;
    }

    [Benchmark]
    public double ManualDotProduct()
    {
        return v1_Original.X * v2_Original.X + v1_Original.Y * v2_Original.Y + v1_Original.Z * v2_Original.Z +
          v3_Original.X * v4_Original.X + v3_Original.Y * v4_Original.Y + v3_Original.Z * v4_Original.Z;
    }

    [Benchmark]
    public double SIMDDotProduct()
    {
        return Vector.Dot(v1_New, v2_New) + Vector.Dot(v3_New, v4_New);
    }

    [Benchmark]
    public MyVector ManualAbs()
    {
        return new MyVector(
            Math.Abs(v1_Original.X) + Math.Abs(v2_Original.X) + Math.Abs(v3_Original.X) + Math.Abs(v4_Original.X),
            Math.Abs(v1_Original.Y) + Math.Abs(v2_Original.Y) + Math.Abs(v3_Original.Y) + Math.Abs(v4_Original.Y),
            Math.Abs(v1_Original.Z) + Math.Abs(v2_Original.Z) + Math.Abs(v3_Original.Z) + Math.Abs(v4_Original.Z)
        );
    }

    [Benchmark]
    public Vector<double> SIMDAbs()
    {
        return Vector.Abs(v1_New) + Vector.Abs(v2_New) + Vector.Abs(v3_New) + Vector.Abs(v4_New);
    }

    [Benchmark]
    public MyVector ManualMin()
    {
        return new MyVector(
            Math.Min(Math.Min(Math.Min(v1_Original.X, v2_Original.X), v3_Original.X), v4_Original.X),
            Math.Min(Math.Min(Math.Min(v1_Original.Y, v2_Original.Y), v3_Original.Y), v4_Original.Y),
            Math.Min(Math.Min(Math.Min(v1_Original.Z, v2_Original.Z), v3_Original.Z), v4_Original.Z)
        );
    }

    [Benchmark]
    public Vector<double> SIMDMin()
    {
        return Vector.Min(Vector.Min(Vector.Min(v1_New, v2_New), v3_New), v4_New);
    }

    [Benchmark]
    public double ManualSumAll()
    {
        return v1_Original.X + v1_Original.Y + v1_Original.Z + 
                v2_Original.X + v2_Original.Y + v2_Original.Z +
                v3_Original.X + v3_Original.Y + v3_Original.Z +
                v4_Original.X + v4_Original.Y + v4_Original.Z;
    }

    [Benchmark]
    public double SIMDSumAll()
    {
        return Vector.Sum(v1_New) + Vector.Sum(v2_New) + Vector.Sum(v3_New) + Vector.Sum(v4_New);
    }

    [Benchmark]
    public bool ManualLessThanOrEqualAll()
    {
        return (v1_Original.X <= v2_Original.X && 
                v1_Original.Y <= v2_Original.Y && 
                v1_Original.Z <= v2_Original.Z) ||
                (v3_Original.X >= v4_Original.X && 
                v3_Original.Y >= v4_Original.Y && 
                v3_Original.Z >= v4_Original.Z); 
    }

    [Benchmark]
    public bool SIMDLessThanOrEqualAll()
    {
        return Vector.LessThanOrEqualAll(v1_New, v2_New) || Vector.GreaterThanOrEqualAll(v3_New, v4_New);
    }
}