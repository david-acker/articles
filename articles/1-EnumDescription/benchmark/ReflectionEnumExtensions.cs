using System.ComponentModel;

namespace EnumDescription.Benchmark;

public static class ReflectionEnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        var attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)
            as DescriptionAttribute[];

        if (attributes is null || attributes.Length == 0)
        {
            return string.Empty;
        }

        return attributes[0].Description;
    }
}