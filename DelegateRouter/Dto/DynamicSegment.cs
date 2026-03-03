namespace RnD.DelegateRouter.Dto;

public class DynamicSegment(string parameterName, Func<string, (bool, object?)> parser) : RouteSegment
{
    private readonly Func<string, (bool success, object? value)> _parser = parser;
    public string ParameterName => parameterName;
    public override bool Match(string segment, out object? value)
    {
        var (success, parsedValue) = _parser(segment);
        value = parsedValue;
        return success;
    }
}
