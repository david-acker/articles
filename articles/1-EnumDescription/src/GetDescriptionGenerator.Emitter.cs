using System.Text;
using EnumDescription.Generators.Models;

namespace EnumDescription.Generators;

public sealed partial class GetDescriptionGenerator
{
    internal sealed class Emitter
    {
        private readonly StringBuilder _builder = new();
        
        public string Emit(IEnumerable<EnumDescriptionExtension> enumExtensions, CancellationToken cancellationToken)
        {
            _builder.Clear();
            _builder.AppendLine("// <auto-generated/>");
            _builder.AppendLine("#nullable enable");

            foreach (var enumExtension in enumExtensions)
            {
                cancellationToken.ThrowIfCancellationRequested();
                GenerateType(enumExtension);
            }
            
            return _builder.ToString();
        }

        private void GenerateType(EnumDescriptionExtension enumExtension)
        {
            _builder.AppendLine($"namespace {enumExtension.Namespace}");
            _builder.AppendLine("{{");
            _builder.AppendLine("    public static partial class EnumExtensions");
            _builder.AppendLine("    {{");

            foreach (var enumExtensionMethod in enumExtension.Methods)
            {
                GenerateMethod(_builder, enumExtensionMethod);
            }
            
            _builder.AppendLine("    }}");
            _builder.AppendLine("}}");
        }

        private static void GenerateMethod(StringBuilder builder, EnumDescriptionExtensionMethod enumExtensionMethod)
        {
            builder.AppendLine($"        public static string GetDescription(this {enumExtensionMethod.EnumName} value)");
            builder.AppendLine("        {");
            builder.AppendLine("            return switch (value)");
            builder.AppendLine("            {");
            
            foreach (var member in enumExtensionMethod.Members)
            {
                builder.AppendLine($"                case {enumExtensionMethod.EnumName}.{member.MemberName}:");
                builder.AppendLine($"                    return \"{member.Value}\";");
            }
            
            builder.AppendLine("                default:");
            builder.AppendLine("                    return string.Empty;");
            
            builder.AppendLine("            {");
            builder.AppendLine("        }");
        }
    }
}
