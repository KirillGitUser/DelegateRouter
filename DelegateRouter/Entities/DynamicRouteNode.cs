namespace RnD.DelegateRouter.Entities;

public class DynamicRouteNode(string parameterName, Func<string, (bool, object?)> parser) : RouteNode
{
    public string ParameterName { get; } = parameterName;

    public bool MatchSegment(string segment, out object? value)
    {
        return parser(segment).Item1 ? (value = parser(segment).Item2, true).Item2 : (value = null, false).Item2;
    }
}

public record RouteMatch(RouteHandler Handler, Dictionary<string, object> Parameters, string MatchedTemplate);
