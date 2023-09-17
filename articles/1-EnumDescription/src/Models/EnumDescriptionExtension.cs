namespace EnumDescription.Generators.Models;

internal sealed class EnumDescriptionExtension
{
    public string Namespace { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public List<EnumDescriptionExtensionMethod> Methods { get; } = new();
    
}

internal sealed class EnumDescriptionExtensionMethod
{
    public string Name { get; set; } = string.Empty;
    
    public string UniqueName { get; set; } = string.Empty;
    
    public string EnumName { get; set; } = string.Empty;
    
    public List<EnumDescriptionMemberData> Members { get; } = new();
}

internal sealed class EnumDescriptionMemberData
{
    public string MemberName { get; set; } = string.Empty;
    
    public string Value { get; set; } = string.Empty;
}