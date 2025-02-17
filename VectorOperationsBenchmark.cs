using System.Numerics;
using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser]
public class VectorOperationsBenchmark
{
    public struct Vector3d
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public Vector3d(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    private Vector3d v1_Manual;
    private Vector3d v2_Manual;
    private Vector<double> v1_SIMD;
    private Vector<double> v2_SIMD;

    [GlobalSetup]
    public void Setup()
    {
        // Setup data for manual and SIMD operations
        v1_Manual = new Vector3d(1.0, 2.0, 3.0);
        v2_Manual = new Vector3d(4.0, 5.0, 6.0);

        v1_SIMD = new Vector<double>(new double[] { 1.0, 2.0, 3.0 });
        v2_SIMD = new Vector<double>(new double[] { 4.0, 5.0, 6.0 });
    }

    [Benchmark]
    public Vector3d ManualAddition()
    {
        return new Vector3d(v1_Manual.X + v2_Manual.X, v1_Manual.Y + v2_Manual.Y, v1_Manual.Z + v2_Manual.Z);
    }

    [Benchmark]
    public Vector<double> SIMDAddition()
    {
        return v1_SIMD + v2_SIMD;
    }

    [Benchmark]
    public double ManualDotProduct()
    {
        return v1_Manual.X * v2_Manual.X + v1_Manual.Y * v2_Manual.Y + v1_Manual.Z * v2_Manual.Z;
    }

    [Benchmark]
    public double SIMDDotProduct()
    {
        return Vector.Dot(v1_SIMD, v2_SIMD);
    }

    [Benchmark]
    public Vector3d ManualMin()
    {
        return new Vector3d(
            v1_Manual.X < v2_Manual.X ? v1_Manual.X : v2_Manual.X,
            v1_Manual.Y < v2_Manual.Y ? v1_Manual.Y : v2_Manual.Y,
            v1_Manual.Z < v2_Manual.Z ? v1_Manual.Z : v2_Manual.Z
        );
    }

    [Benchmark]
    public Vector<double> SIMDMin()
    {
        return Vector.Min(v1_SIMD, v2_SIMD);
    }

    [Benchmark]
    public double ManualSumBoth()
    {
        return v1_Manual.X + v1_Manual.Y + v1_Manual.Z + 
                v2_Manual.X + v2_Manual.Y + v2_Manual.Z;
    }

    [Benchmark]
    public double SIMDSumBoth()
    {
        return Vector.Sum(v1_SIMD) + Vector.Sum(v2_SIMD);
    }

    [Benchmark]
    public bool ManualLessThanOrEqualAll()
    {
        return v1_Manual.X < v2_Manual.X && 
                v1_Manual.Y < v2_Manual.Y && 
                v1_Manual.Z < v2_Manual.Z; 
    }

    [Benchmark]
    public bool SIMDLessThanOrEqualAll()
    {
        return Vector.LessThanOrEqualAll(v1_SIMD, v2_SIMD);
    }
}