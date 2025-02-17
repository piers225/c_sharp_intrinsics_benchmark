

using System.Numerics;
using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser]
public class Vector3Benchmark
{

    [GlobalSetup]
    public void Setup()
    {
        _vectorA = new Vector3() { X = 2.2f, Y = 4.1f, Z = 32 };
        _vectorB = new Vector3() { X = 4.5f, Y = 64f, Z = 93.3f };

        _vectorC = new Vector4() { X = 12, Y = 23f, Z = 42, W = 91.3f };
        _vectorD = new Vector4() { X = 13.3f, Y = 31.1f, Z = 42, W = 91.3f };

    }

    private Vector3 _vectorA;
    private Vector3 _vectorB;

    private Vector4 _vectorC;
    private Vector4 _vectorD;

    [Benchmark(Baseline = true)]
    public Vector3 Vector3_Multiply()
    {
        return _vectorA * _vectorB;
    }

    [Benchmark()]
    public Vector4 Vector4_Multiply()
    {
        return _vectorC * _vectorD;
    }

}