namespace RnD.DelegateRouter.Dto;

public class StaticSegment(string value) : RouteSegment
{
    private readonly string _value = value;
    public override bool Match(string segment, out object? value)
    {
        value = null;
        return string.Equals(_value, segment, StringComparison.Ordinal);
    }
}
