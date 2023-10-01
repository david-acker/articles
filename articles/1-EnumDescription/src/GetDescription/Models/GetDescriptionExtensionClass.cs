using Microsoft.CodeAnalysis;

namespace EnumDescription.Generators.GetDescription.Models;

internal sealed class GetDescriptionExtensionClass
{
    public string Name { get; }
    public string FullName { get; }
    public string Namespace { get; }
    
    public IReadOnlyList<EnumMemberToDescription> EnumMembersToDescriptions { get; }

    public GetDescriptionExtensionClass(
        INamedTypeSymbol enumSymbol, 
        IReadOnlyList<EnumMemberToDescription> enumMembersToDescriptions)
    {
        Name = enumSymbol.Name;
        FullName = enumSymbol.ToString();
        Namespace = enumSymbol.ContainingNamespace.ToString();
        EnumMembersToDescriptions = enumMembersToDescriptions;
    }
}
