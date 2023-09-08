**Article One: Introduction + Generator (Part 1)**

---

### Introduction

.NET's Roslyn platform provides powerful tools for developers to analyze and transform their code during the compilation process. Generators and Analyzers are two such tools that empower developers to enforce coding standards and automate code generation. In this series, we're going to delve deep into the creation of a Roslyn Generator and Analyzer combo aimed at enhancing the usability and maintainability of enums.

Enums in .NET are a great way to represent a set of named constants. However, many times, there's a need for a more friendly or descriptive string than what the enum member provides. This is where the `DescriptionAttribute` shines. But managing these descriptions and ensuring every enum member has one can be a tedious task. Our Roslyn tools will simplify this.

**Generator (Part 1)**

#### Target Attribute Definition

To focus our generator on specific enums, we'll use a target attribute. This ensures that the generator doesn't process every enum in your codebase but only those you specifically mark. Let's define our target attribute:

```csharp
[AttributeUsage(AttributeTargets.Enum, Inherited = false)]
public sealed class DescriptiveEnumAttribute : Attribute
{
    public DescriptiveEnumAttribute() { }
}
```

Enums marked with `[DescriptiveEnum]` will be targeted by our generator.

#### Setting up the Generator

Start by creating a new class library project that targets .NET Standard 2.0 or higher. Add the `Microsoft.CodeAnalysis.CSharp` and `Microsoft.CodeAnalysis.Analyzers` NuGet packages.

Define an initial generator by implementing the `ISourceGenerator` interface.

```csharp
[Generator]
public class DescriptiveEnumGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Will be filled in part 2
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Will be filled in part 2
    }
}
```

This generator, once complete, will search for enums marked with our `DescriptiveEnumAttribute` and generate switch-based methods to return the description of the enum members.

#### Scanning the Code

Within the `Execute` method of our generator, we'll scan for the target enums:

```csharp
public void Execute(GeneratorExecutionContext context)
{
    var compilation = context.Compilation;

    // Search for enums with our attribute
    var descriptiveEnums = compilation.SyntaxTrees
        .SelectMany(tree => tree.GetRoot().DescendantNodes())
        .OfType<EnumDeclarationSyntax>()
        .Where(enumDeclaration => enumDeclaration.AttributeLists
            .Any(al => al.Attributes
                .Any(attr => attr.Name.ToString().Contains("DescriptiveEnum"))));
}
```

#### Generating Code

For each descriptive enum we find, we'll generate a switch-based method:

```csharp
foreach (var enumDeclaration in descriptiveEnums)
{
    var enumName = enumDeclaration.Identifier.ValueText;
    var namespaceName = (enumDeclaration.Parent as NamespaceDeclarationSyntax)?.Name.ToString();

    var switchCases = new StringBuilder();

    foreach (var member in enumDeclaration.Members)
    {
        var descriptionAttr = member.AttributeLists
            .SelectMany(al => al.Attributes)
            .FirstOrDefault(attr => attr.Name.ToString().Contains("Description"));

        var descriptionValue = descriptionAttr?.ArgumentList?.Arguments[0].ToString() ?? $"\"{member.Identifier.ValueText}\"";

        switchCases.AppendLine($"    case {enumName}.{member.Identifier.ValueText}: return {descriptionValue};");
    }

    var generatedCode = $@"
namespace {namespaceName}
{{
    public static class {enumName}Extensions
    {{
        public static string GetDescription(this {enumName} value)
        {{
            switch (value)
            {{
{switchCases}
                default: throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }}
        }}
    }}
}}
";
    context.AddSource($"{enumName}Extensions", SourceText.From(generatedCode, Encoding.UTF8));
}
```

In the next article, we'll complete our analyzer, ensuring it raises warnings for missing `DescriptionAttribute`s. Plus, we'll wrap everything up and show you how to package your Roslyn tools into a handy NuGet package for reuse.

[Read on to discover the magic of Analyzers in C#!](/articles/1-EnumDescription/drafts/3-Analyzer.md)
