using RnD.DelegateRouter.Helpers;

namespace RnD.DelegateRouter.Entities;

public class RouteNode
{
    private readonly Dictionary<string, RouteNode> _staticChildren = [];
    private readonly List<DynamicRouteNode> _dynamicChildren = [];
    private RouteHandler? _handler;

    public void AddRoute(string[] segments, int index, RouteHandler handler)
    {
        if (index >= segments.Length)
        {
            _handler = handler;
            return;
        }

        var segment = segments[index];

        if (segment.StartsWith('{') && segment.EndsWith('}'))
        {
            var (paramName, type) = ParseDynamicSegment(segment);
            var dynamicNode = _dynamicChildren.FirstOrDefault(n => n.ParameterName == paramName);

            if (dynamicNode == null)
            {
                dynamicNode = new DynamicRouteNode(paramName, GetParser(type));
                _dynamicChildren.Add(dynamicNode);
            }

            dynamicNode.AddRoute(segments, index + 1, handler);
        }
        else
        {
            if (!_staticChildren.TryGetValue(segment, out var staticNode))
            {
                staticNode = new RouteNode();
                _staticChildren[segment] = staticNode;
            }

            staticNode.AddRoute(segments, index + 1, handler);
        }
    }

    public RouteMatch? Match(string[] segments, int index, Dictionary<string, object> parameters)
    {
        if (index >= segments.Length)
        {
            return _handler != null ? new RouteMatch(_handler, parameters) : null;
        }

        var currentSegment = segments[index];

        if (_staticChildren.TryGetValue(currentSegment, out var staticNode))
        {
            var result = staticNode.Match(segments, index + 1, parameters);
            if (result != null)
            {
                return result;
            }
        }

        foreach (var dynamicNode in _dynamicChildren)
        {
            if (dynamicNode.MatchSegment(currentSegment, out var value))
            {
                Dictionary<string, object> newParams = new(parameters)
                {
                    [dynamicNode.ParameterName] = value!
                };

                var result = dynamicNode.Match(segments, index + 1, newParams);
                if (result != null)
                {
                    return result;
                }
            }
        }

        return null;
    }

    private static (string name, string type) ParseDynamicSegment(string segment)
    {
        var content = segment.Trim('{', '}');
        var parts = content.Split(':');
        return (parts[0], parts[1]);
    }

    private static Func<string, (bool, object?)> GetParser(string type) => type.ToLower() switch
    {
        "bool" => SegmentParsers.ParseBool,
        "int" => SegmentParsers.ParseInt,
        "guid" => SegmentParsers.ParseGuid,
        "string" => SegmentParsers.ParseString,
        "datetime" => SegmentParsers.ParseDateTime,
        "float" => SegmentParsers.ParseFloat,
        "double" => SegmentParsers.ParseDouble,
        "decimal" => SegmentParsers.ParseDecimal,
        "long" => SegmentParsers.ParseLong,
        "timespan" => SegmentParsers.ParseTimeSpan,
        _ => throw new NotSupportedException($"Type {type} is not supported")
    };
}
