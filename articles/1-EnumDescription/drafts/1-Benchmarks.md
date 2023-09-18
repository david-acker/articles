# Enum Description: Benchmarking

## Table of Contents

- [Scenario](#scenario)
- [Reflection-Based Approach](#reflection-based-approach)
- [Switch-Based Approach](#switch-based-approach)
- [Performance Benchmarking](#performance-benchmarking)
- [Comparison](#comparison)
- [Next Steps](#next-steps)

## Scenario

When working with enums in C#, it's common want to associate a string description with each enum member.

For example, suppose we have a `Country` enum. The individual enum members could be `US`, `CA`, and `UK`. We may want to associate the descriptions `United States`, `Canada`, and `United Kingdom` with these enum members, respectively.

## Reflection-Based Approach

One method of achieving this is by adding an attribute with the string description to each enum member, and then using reflection to access the description values at runtime. In this approach, the `DescriptionAttribute` from `System.ComponentModel` is utilized to associate descriptions with enum members.

```csharp
using System.ComponentModel;

public enum Country
{
    [Description("United States")]
    US,

    [Description("Canada")]
    CA,

    [Description("United Kingdom")]
    UK
}
```

Using reflection, these descriptions can be retrieved at runtime.

```csharp
using System;
using System.Reflection;

public static class ReflectionEnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

        var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
            as DescriptionAttribute[];

        if (attributes is not null && attributes.Length > 0)
        {
            return attributes[0].Description;
        }
        else
        {
            return value.ToString();
        }
    }
}
```

## Switch-Based Approach

An alternative is to use a dedicated method containing a switch statement to associate a description with each enum member. This approach will be more performant, but will require additional code changes to keep the `GetDescription` method in sync when adding or updating enum members.

```csharp
public static class SwitchEnumExtensions
{
    public static string GetDescription(this Country value)
    {
        switch (value)
        {
            case Country.US:
                return "United States";
            case Country.CA:
                return "Canada";
            case Country.UK:
                return "United Kingdom";
            default:
                return string.Empty;
        }
    }
}
```

## Performance Benchmarking

To compare the performance of these two methods, we'll use `Benchmark.NET`, a .NET library for benchmarking.

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class BenchmarkRunner
{
    [Benchmark]
    public void ReflectionBasedDescription()
    {
        var description = Country.UK.GetDescription();
    }

    [Benchmark]
    public void SwitchBasedDescription()
    {
        var description = Country.UK.GetDescription();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BenchmarkRunner>();
    }
}
```

### Results

| Method        |        Mean |      Error |    StdDev |   Gen0 | Allocated |
| ------------- | ----------: | ---------: | --------: | -----: | --------: |
| ViaReflection | 610.0784 ns | 10.6254 ns | 9.9390 ns | 0.1259 |     264 B |
| ViaSwitch     |   0.8571 ns |  0.0017 ns | 0.0016 ns |      - |         - |

The switch-based approach outperforms the reflection-based approach not only in execution time but also in memory allocation. While these performance improvements are likely negligible for most applications, the switch-based approach has an additional benefit of being compatible with applications using Ahead-of-Time (AOT) compilation, where the use of reflection is either limited or unsupported.

## Next Steps

Next, we'll delve into the world of source generators, which will allow us the craft a solution that boasts the performance and compatibility of the switch-based approach, without sacrificing the flexibility and maintainability of the reflection-based method.
