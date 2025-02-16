using BenchmarkDotNet.Attributes;

[DisassemblyDiagnoser]
public partial class Flags
{
    [Flags]
    public enum Colors
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 4,
        Yellow = 8
    }

    Colors selectedColors = Colors.Red | Colors.Green;

    [Benchmark]
    public bool HasFlag() => selectedColors.HasFlag(Colors.Red);
}