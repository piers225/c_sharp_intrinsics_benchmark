using System.Numerics;
using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser]
public class VectorAddBenchmark
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

    [GlobalSetup]
    public void Setup()
    {
        _vector3dA = new Vector3d( X : 2, Y : 2.2, Z : 4.4 );
        _vector3dB = new Vector3d( X : 32, Y : 2.2, Z : 4.4 );

        _vectorA = Vector.Create<double>([2, 2.2, 4.4, 0]);
        _vectorB = Vector.Create<double>([32, 2.2, 4.4, 0]);
    }

    private Vector3d _vector3dA;
    private Vector3d _vector3dB;
    private Vector<double> _vectorA;
    private Vector<double> _vectorB;
    

    [Benchmark(Baseline = true)]
    public Vector3d Vector3dAdd()
    {
        return new Vector3d(
            X  : _vector3dA.X + _vector3dB.X,
            Y : _vector3dA.Y + _vector3dB.Y,
            Z  : _vector3dA.Z + _vector3dB.Z
        );
    }

    [Benchmark]
    public Vector<double> VectorAdd()
    {
        return _vectorA + _vectorB;
    }
}