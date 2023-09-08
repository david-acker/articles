using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EnumDescription.Generators;

public sealed partial class GetDescriptionGenerator
{
    internal sealed class Parser
    {
        internal const string EnumDescriptionAttribute = "EnumDescription.EnumDescriptionAttribute";

        private readonly Compilation _compilation;
        private readonly Action<Diagnostic> _reportDiagnostic;
        private readonly CancellationToken _cancellationToken;

        public Parser(Compilation compilation, Action<Diagnostic> reportDiagnostic, CancellationToken cancellationToken)
        {
            _compilation = compilation;
            _reportDiagnostic = reportDiagnostic;
            _cancellationToken = cancellationToken;
        }

        public IReadOnlyList<EnumExtension> GetEnumExtensions(IEnumerable<EnumDeclarationSyntax> enumDeclarations)
        {
            var enumExtensionsToGenerate = new List<EnumExtension>();

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
                
                ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
                
                var memberData = new List<EnumMemberData>(enumMembers.Length);
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
                        
                        memberData.Add(new EnumMemberData
                        {
                            MemberName = member.Name,
                            Value = descriptionValue
                        });
                    }
                }

                if (canGenerate)
                {
                    var enumExtensionMethod = new EnumExtensionMethod
                    {
                        Name = "GetDescription",
                        // TODO: Make unique
                        UniqueName = "GetDescription",
                        EnumName = enumSymbol.ToString()
                    };
                    enumExtensionMethod.Members.AddRange(memberData);
                }
            }

            return enumExtensionsToGenerate;
        }
        
        private void ReportDiagnostic(DiagnosticDescriptor diagnostic, Location? location, params object?[]? messageArgs)
        {
            _reportDiagnostic(Diagnostic.Create(diagnostic, location, messageArgs));
        }

        internal sealed class EnumExtension
        {
            public string Namespace = string.Empty;
            public string Name = string.Empty;
            public readonly List<EnumExtensionMethod> Methods = new();
        }

        internal sealed class EnumExtensionMethod
        {
            public string Name = string.Empty;
            public string UniqueName = string.Empty;
            public string EnumName = string.Empty;
            public readonly List<EnumMemberData> Members = new();
        }

        internal sealed class EnumMemberData
        {
            public string MemberName = string.Empty;
            public string Value = string.Empty;
        }
    }
}
