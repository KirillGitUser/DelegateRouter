namespace RnD.DelegateRouter.Dto;

public abstract class RouteSegment
{
    public abstract bool Match(string segment, out object? value);
}
