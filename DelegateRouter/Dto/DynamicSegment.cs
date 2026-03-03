namespace RnD.DelegateRouter.Dto;

public class DynamicSegment(Func<string, (bool, object?)> parser) : RouteSegment
{
    public override bool Match(string segment, out object? value)
    {
        var (success, parsedValue) = parser(segment);
        value = parsedValue;
        return success;
    }
}
