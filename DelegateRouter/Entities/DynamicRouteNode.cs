namespace RnD.DelegateRouter.Entities;

public class DynamicRouteNode(string parameterName, Func<string, (bool isOk, object? value)> parser) : RouteNode
{
    public string ParameterName { get; } = parameterName;

    public bool MatchSegment(string segment, out object? value)
    {
        return parser(segment).isOk ? (value = parser(segment).value, true).Item2 : (value = null, false).Item2;
    }
}

public record RouteMatch(RouteHandler Handler, Dictionary<string, object> Parameters, string MatchedTemplate);
