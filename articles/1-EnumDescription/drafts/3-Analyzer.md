**Analyzer (Part 1)**

Our analyzer will ensure that every member of an enum, marked with the `DescriptiveEnumAttribute`, also carries a `DescriptionAttribute`.

Start by creating an `enumAnalyzer` that will check our conditions:

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EnumDescriptionAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        "ENUM01",
        "Missing DescriptionAttribute",
        "DescriptionAttribute coverage must be exhaustive",
        "EnumRules",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeEnumDeclaration, SyntaxKind.EnumDeclaration);
    }
}

private void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
{
    // This will be filled in Analyzer (Part 2)
}
```

---

In the next article, we'll complete our analyzer, ensuring it raises warnings for missing `DescriptionAttribute`s. Plus, we'll wrap everything up and show you how to package your Roslyn tools into a handy NuGet package for reuse.

**Analyzer (Part 2)**

Continuing with our analyzer, our goal is to raise warnings for enums with the `DescriptiveEnumAttribute` but without exhaustive `DescriptionAttribute` coverage on all its members.

### Finalizing the Analyzer

We left off with the `AnalyzeEnumDeclaration` method pending. Let's complete that:

```csharp
private void AnalyzeEnumDeclaration(SyntaxNodeAnalysisContext context)
{
    var enumDeclaration = (EnumDeclarationSyntax)context.Node;
    var hasDescriptiveAttribute = enumDeclaration.AttributeLists
        .SelectMany(al => al.Attributes)
        .Any(attr => attr.Name.ToString().Contains("DescriptiveEnum"));

    if (!hasDescriptiveAttribute)
        return;

    foreach (var member in enumDeclaration.Members)
    {
        var hasDescription = member.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(attr => attr.Name.ToString().Contains("Description"));

        if (!hasDescription)
        {
            var diagnostic = Diagnostic.Create(Rule, member.GetLocation(), member.Identifier.ValueText);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
```

Now, the analyzer will loop through all the members of an enum marked with `DescriptiveEnumAttribute` and report a diagnostic for every member that doesn't have a `DescriptionAttribute`.

[Read on to discover the magic of NuGet in C#!](/articles/1-EnumDescription/drafts/4-Packaging.md)
