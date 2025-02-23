
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Running;

public static class Program
{
    public static void Main(string[] args) 
    {
        Console.WriteLine($"Vector IsHardwareAccelerated - {Vector.IsHardwareAccelerated}");

        Console.WriteLine($"Sse IsSupported - {Sse.IsSupported}");
        Console.WriteLine($"Sse2 IsSupported - {Sse2.IsSupported}");
        Console.WriteLine($"Sse3 IsSupported - {Sse3.IsSupported}");
        Console.WriteLine($"Sse41 IsSupported - {Sse41.IsSupported}");
        Console.WriteLine($"Sse42 IsSupported - {Sse42.IsSupported}");

        Console.WriteLine($"Avx IsSupported - {Avx.IsSupported}");
        Console.WriteLine($"Avx2 IsSupported - {Avx2.IsSupported}");
        Console.WriteLine($"Avx10v1 IsSupported - {Avx10v1.IsSupported}");
        Console.WriteLine($"Avx512BW IsSupported - {Avx512BW.IsSupported}");
        Console.WriteLine($"Avx512CD IsSupported - {Avx512CD.IsSupported}");
        Console.WriteLine($"Avx512DQ IsSupported - {Avx512DQ.IsSupported}");
        Console.WriteLine($"Avx512F IsSupported - {Avx512F.IsSupported}");
        Console.WriteLine($"Avx512Vbmi IsSupported - {Avx512Vbmi.IsSupported}");
        Console.WriteLine($"AvxVnni IsSupported - {AvxVnni.IsSupported}");

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}