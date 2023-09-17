using System.Collections.Immutable;
using EnumDescription.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EnumDescription.Generators;

public sealed partial class GetDescriptionGenerator
{
    internal sealed class Parser
    {
        internal const string EnumDescriptionAttribute = "EnumDescription.Generators.EnumDescriptionAttribute";

        private readonly Compilation _compilation;
        private readonly Action<Diagnostic> _reportDiagnostic;
        private readonly CancellationToken _cancellationToken;

        public Parser(Compilation compilation, Action<Diagnostic> reportDiagnostic, CancellationToken cancellationToken)
        {
            _compilation = compilation;
            _reportDiagnostic = reportDiagnostic;
            _cancellationToken = cancellationToken;
        }

        public IEnumerable<EnumDescriptionExtension> GetEnumExtensions(IEnumerable<EnumDeclarationSyntax> enumDeclarations)
        {
            var enumExtensionsToGenerate = new List<EnumDescriptionExtension>();

            var descriptionAttribute = _compilation.GetTypeByMetadataName("System.ComponentModel.DescriptionAttribute");
            if (descriptionAttribute is null)
            {
                return enumExtensionsToGenerate;
            }
            
            foreach (var enumDeclaration in enumDeclarations)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                
                var semanticModel = _compilation.GetSemanticModel(enumDeclaration.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(enumDeclaration) is not INamedTypeSymbol enumSymbol)
                {
                    continue;
                }
                
                ImmutableArray<IFieldSymbol> enumMembers = enumSymbol.GetMembers()
                    .OfType<IFieldSymbol>()
                    .ToImmutableArray();
                
                var memberData = new List<EnumDescriptionMemberData>(enumMembers.Length);
                var canGenerate = true;
                
                // Get all the fields from the enum, and add their name to the list
                foreach (var member in enumMembers)
                {
                    if (member is IFieldSymbol { ConstantValue: not null } fieldSymbol)
                    {
                        var attributes = fieldSymbol.GetAttributes();
                        var memberDescriptionAttribute = attributes.FirstOrDefault(attribute =>
                            SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, descriptionAttribute));
                        
                        if (memberDescriptionAttribute is null
                            || memberDescriptionAttribute.ConstructorArguments.Length != 1
                            || memberDescriptionAttribute.ConstructorArguments[0].Value is not string descriptionValue)
                        {
                            canGenerate = false;
                            // Report diagnostic

                            ReportDiagnostic(
                                DiagnosticDescriptors.MissingDescriptionAttribute,
                                member.Locations.FirstOrDefault());

                            continue;
                        }
                        
                        memberData.Add(new EnumDescriptionMemberData
                        {
                            MemberName = member.Name,
                            Value = descriptionValue
                        });
                    }
                }

                if (canGenerate)
                {
                    var enumExtensionMethod = new EnumDescriptionExtensionMethod
                    {
                        Name = "GetDescription",
                        // TODO: Make unique
                        UniqueName = "GetDescription",
                        EnumName = enumSymbol.ToString()
                    };
                    enumExtensionMethod.Members.AddRange(memberData);

                    var extension = new EnumDescriptionExtension();
                    extension.Namespace = enumSymbol.ContainingNamespace.ToString();
                    extension.Methods.Add(enumExtensionMethod);
                    extension.Name = "EnumExtensions";

                    enumExtensionsToGenerate.Add(extension);
                }
            }

            return enumExtensionsToGenerate;
        }
        
        private void ReportDiagnostic(DiagnosticDescriptor diagnostic, Location? location, params object?[]? messageArgs)
        {
            _reportDiagnostic(Diagnostic.Create(diagnostic, location, messageArgs));
        }
    }
}
