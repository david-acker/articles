using System.ComponentModel;

namespace EnumDescription.Benchmark;

public enum Color
{
    [Description("Bright Red")]
    Red,
    
    [Description("Grassy Green")]
    Green,
    
    [Description("Cool Blue")]
    Blue
}

public static class ReflectionEnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());

        if (fieldInfo is not null)
        {
            var attributes = 
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
        }

        return value.ToString();
    }
}


public static class SwitchEnumExtensions
{
    public static string GetDescription(this Color value)
    {
        return value switch
        {
            Color.Red => "Bright Red",
            Color.Green => "Grassy Green",
            Color.Blue => "Cool Blue",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
