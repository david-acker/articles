using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EnumDescription.Generators;

[Generator]
public sealed partial class GetDescriptionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<EnumDeclarationSyntax> enumDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                Parser.EnumDescriptionAttribute,
                static (node, _) => IsTargetForGeneration(node),
                static (syntaxContext, _) => GetTargetForGeneration(syntaxContext))
            .Where(static node => node is not null)!;

        IncrementalValueProvider<(Compilation, ImmutableArray<EnumDeclarationSyntax>)> compilationAndEnums =
            context.CompilationProvider.Combine(enumDeclarations.Collect());
        
        context.RegisterSourceOutput(compilationAndEnums,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static bool IsTargetForGeneration(SyntaxNode node)
    {
        return node is EnumDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private static EnumDeclarationSyntax? GetTargetForGeneration(GeneratorAttributeSyntaxContext context)
    {
        var enumDeclaration = (EnumDeclarationSyntax)context.TargetNode;

        foreach (var attributeList in enumDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol
                    attributeConstructorSymbol)
                {
                    continue;
                }

                var attributeSymbol = attributeConstructorSymbol.ContainingType;
                var attributeSymbolName = attributeSymbol.ToDisplayString();

                if (attributeSymbolName == Parser.EnumDescriptionAttribute)
                {
                    return enumDeclaration;
                }
            }
        }

        return null;
    }
    
    private static void Execute(
        Compilation compilation, 
        ImmutableArray<EnumDeclarationSyntax> enums,
        SourceProductionContext context)
    {
        if (enums.IsDefaultOrEmpty)
        {
            return;
        }
        
        var parser = new Parser(compilation, context.ReportDiagnostic, context.CancellationToken);
        var enumExtensions = parser.GetEnumExtensions(enums.Distinct());

        if (enums.Length == 0)
        {
            return;
        }
        
        var emitter = new Emitter();
        var result = emitter.Emit(enumExtensions, context.CancellationToken);
        
        context.AddSource("EnumDescriptionExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
    }
}
