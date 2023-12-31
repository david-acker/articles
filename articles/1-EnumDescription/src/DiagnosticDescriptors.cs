using Microsoft.CodeAnalysis;

namespace EnumDescription.Generators;

internal static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor MissingDescriptionAttribute { get; } = new(
        id: "EDE0001",
        title: "Missing DescriptionAttribute",
        messageFormat: "Enum member is missing DescriptionAttribute",
        category: "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}