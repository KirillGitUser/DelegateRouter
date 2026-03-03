using RnD.DelegateRouter.Entities;

namespace RnD.DelegateRouter.Services;

public interface IRouterService
{
    void RegisterRoute<TDelegate>(string template, TDelegate handler) where TDelegate : Delegate;
    Task<RouteResult> RouteResultAsync(string path, CancellationToken cancellationToken = default);
}
