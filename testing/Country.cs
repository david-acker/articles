using System.ComponentModel;

namespace Testing.Enums;

public enum Country
{
    [Description("United States")]
    US,

    [Description("United Kingdom")]
    UK
}

public class Test
{
    public Test()
    {
        var country = Country.US;
        var description = country.GetDescription();
        //var value = description.GetValueFromDescription<Country>();
    }
}