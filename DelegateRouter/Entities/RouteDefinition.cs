namespace RnD.DelegateRouter.Entities;

public class RouteDefinition
{
    public string Template { get; set; } = string.Empty;
    public RouteHandler Handler { get; set; } = null!;
    public List<RouteSegmentDefinition> Segments { get; set; } = new();
    public DateTime RegisteredAt { get; set; }

    public override string ToString() => $"{Template} -> {Handler.GetParametersInfo()}";
}

public class RouteSegmentDefinition
{
    public string OriginalValue { get; set; } = string.Empty;
    public bool IsDynamic { get; set; }
    public string? ParameterName { get; set; }
    public string? ParameterType { get; set; }
}
