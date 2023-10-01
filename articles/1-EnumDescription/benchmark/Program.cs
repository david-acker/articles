using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using EnumDescription.Benchmark;

[MemoryDiagnoser]
public class EnumExtensionsBenchmark
{
    [Benchmark]
    public string ViaReflection()
    {
        return ReflectionEnumExtensions.GetDescription(Country.UK);
    }

    [Benchmark]
    public string ViaSwitch()
    {
        return Country.UK.GetDescription();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<EnumExtensionsBenchmark>();
    }
}