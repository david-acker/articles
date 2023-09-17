namespace EnumDescription.Generators.Tests;

public sealed class GetDescriptionGeneratorTests
{
    [Fact]
    public void ItWillGenerateGetDescriptionExtension()
    {
        const string source = $@"
using System.ComponentModel;
using EnumDescription.Generators;

namespace Test.Enums;

[EnumDescription]
public enum Countries
{{
    [Description(""United States"")]
    US,

    [Description(""Canada"")]
    CA,

    [Description(""United Kingdom"")]
    UK
}}
";

        var generated = @"
// <auto-generated/>
#nullable enable
namespace Test.Enums
{{
    public static partial class EnumExtensions
    {{
        public static string GetDescription(this Test.Enums.Countries value)
        {
            return switch (value)
            {
                case Test.Enums.Countries.US:
                    return ""United States"";
                case Test.Enums.Countries.CA:
                    return ""Canada"";
                case Test.Enums.Countries.UK:
                    return ""United Kingdom"";
                default:
                    return string.Empty;
            {
        }
    }}
}}";
        
        var (diagnostics, output) = TestHelpers.Generate<GetDescriptionGenerator>(source);
        
        Assert.Empty(diagnostics);
        Assert.Equal(generated.Trim(), output.Trim(), ignoreLineEndingDifferences: true);
    }
}