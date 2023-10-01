namespace EnumDescription.Generators.GetDescription.Models;

internal sealed class EnumMemberToDescription
{
    public string MemberName { get; }
    public string Value { get; }

    public EnumMemberToDescription(string memberName, string value)
    {
        MemberName = memberName;
        Value = value;
    }
}
