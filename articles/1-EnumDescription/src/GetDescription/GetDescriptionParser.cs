using System.Collections.Immutable;
using EnumDescription.Generators.GetDescription.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EnumDescription.Generators.GetDescription;

internal static class GetDescriptionParser
{
    public static GetDescriptionExtensionClass? GetEnumExtensionClass(
        EnumDeclarationSyntax enumDeclaration, 
        Compilation compilation, 
        Action<Diagnostic> reportDiagnostic,
        CancellationToken cancellationToken)
    {
        var loggerMessageAttribute = compilation.GetTypeByMetadataName(WellKnownTypes.EnumDescriptionAttribute);
        if (loggerMessageAttribute is null)
        {
            return null;
        }
        
        var descriptionAttribute = compilation.GetTypeByMetadataName(WellKnownTypes.DescriptionAttribute);
        if (descriptionAttribute is null)
        {
            return null;
        }
        
        cancellationToken.ThrowIfCancellationRequested();
        
        var semanticModel = compilation.GetSemanticModel(enumDeclaration.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(enumDeclaration) is not INamedTypeSymbol enumSymbol)
        {
            return null;
        }
        
        var enumMembers = enumSymbol.GetMembers()
            .OfType<IFieldSymbol>()
            .ToImmutableArray();

        var enumMemberToDescriptions = new List<EnumMemberToDescription>(enumMembers.Length);
        
        foreach (var enumMember in enumMembers)
        {
            if (GetDescriptionValue(enumMember, descriptionAttribute) is not { } descriptionValue)
            {
                reportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.MissingDescriptionAttribute, 
                        enumMember.Locations.FirstOrDefault()));
                
                continue;
            }

            enumMemberToDescriptions.Add(
                new EnumMemberToDescription(enumMember.Name, descriptionValue));
        }

        return new GetDescriptionExtensionClass(enumSymbol, enumMemberToDescriptions);
    }

    private static string? GetDescriptionValue(IFieldSymbol enumMember, INamedTypeSymbol descriptionAttribute)
    {
        var memberDescriptionAttribute = enumMember.GetAttributes()
            .FirstOrDefault(attribute => 
                SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, descriptionAttribute));
        
        if (memberDescriptionAttribute is { ConstructorArguments.Length: 1 }
            && memberDescriptionAttribute.ConstructorArguments[0].Value is string descriptionValue)
        {
            return descriptionValue;
        }

        return null;
    }
}
