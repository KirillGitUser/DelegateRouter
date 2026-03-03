using RnD.DelegateRouter.Entities;

namespace RnD.DelegateRouter.Services;

public class RouterService : IRouterService
{
    private readonly RouteNode _root = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public void RegisterRoute<TDelegate>(string template, TDelegate handler) where TDelegate : Delegate
    {
        var segments = ParseTemplate(template);
        var parameterTypes = GetParameterTypes(handler);
        var routeHandler = new RouteHandler(handler, parameterTypes);

        _lock.EnterWriteLock();
        try
        {
            _root.AddRoute(segments, 0, routeHandler);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public async Task<RouteResult> RouteResultAsync(string path, CancellationToken cancellationToken = default)
    {
        var segments = ParsePath(path);

        _lock.EnterReadLock();

        try
        {
            var match = _root.Match(segments, 0, []);

            if (match == null)
                return RouteResult.Fail($"No route found for path: {path}");

            return 
                await ExecuteHandlerAsync(match.Handler, match.Parameters, cancellationToken);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    private static async Task<RouteResult> ExecuteHandlerAsync(RouteHandler handler, Dictionary<string, object> parameters, CancellationToken cancellationToken)
    {
        try
        {
            var delegateParams = handler.HandlerDelegate.Method.GetParameters();
            var callArgs = new object?[delegateParams.Length];

            for (int i = 0; i < delegateParams.Length; i++)
            {
                var paramName = delegateParams[i].Name;
                callArgs[i] = 
                    paramName != null && parameters.TryGetValue(paramName, out var value) ? 
                        value : 
                        delegateParams[i].DefaultValue;
            }

            var result = handler.HandlerDelegate.DynamicInvoke(callArgs);

            if (result is Task task)
            {
                await task.ConfigureAwait(false);

                var taskType = task.GetType();
                if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var resultProperty = taskType.GetProperty("Result");
                    return RouteResult.Success(resultProperty?.GetValue(task));
                }

                return RouteResult.Success();
            }

            return RouteResult.Success(result);
        }
        catch (Exception ex)
        {
            return RouteResult.Fail($"Handler execution failed: {ex.Message}");
        }
    }

    private static string[] ParseTemplate(string template) => template.Split('/', StringSplitOptions.RemoveEmptyEntries);

    private static string[] ParsePath(string path) => path.Split('/', StringSplitOptions.RemoveEmptyEntries);

    private static Type[] GetParameterTypes(Delegate handler) => [.. handler.Method.GetParameters().Select(p => p.ParameterType)];
}
