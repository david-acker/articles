using BenchmarkDotNet.Attributes;

namespace EnumDescription.Benchmark;

[MemoryDiagnoser]
public class EnumExtensionsBenchmark
{
    [Benchmark]
    public string ViaReflection()
    {
        return ReflectionEnumExtensions.GetDescription(Color.Red);
    }

    [Benchmark]
    public string ViaSwitch()
    {
        return Color.Blue.GetDescription();
    }
}