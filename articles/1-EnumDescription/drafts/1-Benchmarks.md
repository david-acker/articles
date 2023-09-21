# Enum Description: Benchmarking

## Table of Contents

- [Scenario](#scenario)
- [Reflection-Based Approach](#reflection-based-approach)
- [Switch-Based Approach](#switch-based-approach)
- [Performance Benchmarking](#performance-benchmarking)
- [Comparison](#comparison)
- [Next Steps](#next-steps)

## Scenario

In software development, it's a common requirement to associate a string description with each member of an enum.

For example, suppose we have a `Country` enum. The individual enum members could be `US`, `CA`, and `UK`. We may want to associate the descriptions `United States`, `Canada`, and `United Kingdom` with these enum members, respectively.

However, unlike languages such as [Swift](https://www.swift.org) and [TypeScript](https://www.typescriptlang.org), C# does not natively support string-based enums. Let's consider some workarounds:

## Reflection-Based Approach

One way to achieve this is by adding an attribute with the string description to each enum member, and then using reflection to access the description values at runtime.

In following example, the `DescriptionAttribute` from `System.ComponentModel` is utilized to associate descriptions with enum members.

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

The following `GetDescription` extension method then uses reflection to retrieve the description values at runtime.

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

An alternative is a dedicated method that uses a switch statement or expression to associate a description with each enum member.

```csharp
public static class SwitchEnumExtensions
{
    public static string GetDescription(this Country value)
    {
        return value switch
        {
            Country.US => "United States",
            Country.CA => "Canada",
            Country.UK => "United Kingdom",
            _ => string.Empty,
        };
    }
}
```

This approach is more performant, but will require additional code changes to keep the `GetDescription` method in sync when adding or updating enum members.

## Performance Benchmarking

To compare the performance of these two methods, we'll use [`Benchmark.NET`](https://github.com/dotnet/BenchmarkDotNet), a .NET library for benchmarking.

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

The switch-based approach outperforms the reflection-based approach not only in execution time but also in memory allocation. While these performance improvements are likely negligible for most applications, the switch-based approach has the additional benefit of being compatible with Ahead-of-Time (AOT) compilation, where the usage of reflection is either limited or unsupported.

## Next Steps

Next, we'll dive into source generators to create a solution which combines the benefits of both the switch-based approach and the reflection-based approach.
