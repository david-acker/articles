using System.ComponentModel;

namespace EnumDescription.Benchmark;

public enum Country
{
    [Description("United States")]
    US,

    [Description("Canada")]
    CA,

    [Description("United Kingdom")]
    UK
}