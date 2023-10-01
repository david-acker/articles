namespace EnumDescription.Benchmark;

public static class SwitchEnumExtensions
{
    public static string GetDescription(this Country value)
    {
        return value switch
        {
            Country.US => "United States",
            Country.CA => "Canada",
            Country.UK => "United Kingdom",
            _ => string.Empty,
        };
    }
}