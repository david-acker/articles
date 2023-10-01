using System.Text;
using EnumDescription.Generators.GetDescription;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EnumDescription.Generators;

[Generator]
public sealed class EnumDescriptionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                WellKnownTypes.EnumDescriptionAttribute,
                static (node, _) => node is EnumDeclarationSyntax,
                static (ctx, _) => ctx.TargetNode as EnumDeclarationSyntax)
            .Where(static node => node is not null);

        IncrementalValuesProvider<(EnumDeclarationSyntax?, Compilation)> enumDeclarationsAndCompilation =
            enumDeclarations.Combine(context.CompilationProvider);

        context.RegisterSourceOutput(enumDeclarationsAndCompilation,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static void Execute(
        EnumDeclarationSyntax? enumDeclaration,
        Compilation compilation, 
        SourceProductionContext context)
    {
        if (enumDeclaration is null)
        {
            return;
        }
        
        var enumExtensionClass = GetDescriptionParser.GetEnumExtensionClass(
            enumDeclaration,
            compilation,
            context.ReportDiagnostic,
            context.CancellationToken);

        if (enumExtensionClass is null)
        {
            return;
        }
        
        context.CancellationToken.ThrowIfCancellationRequested();
        
        var (generatedSource, fileName) = GetDescriptionEmitter.Emit(enumExtensionClass);
        context.AddSource(fileName, SourceText.From(generatedSource, Encoding.UTF8));
    }
}
