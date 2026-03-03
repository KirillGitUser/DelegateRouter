using RnD.DelegateRouter.Entities;
using System.Collections.Concurrent;

namespace RnD.DelegateRouter.Services;

public class RouterService : IRouterService
{
    private readonly RouteNode _root = new();
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);
    private readonly ConcurrentDictionary<string, RouteDefinition> _routeRegistry = new();

    public void RegisterRoute<TDelegate>(string template, TDelegate handler) where TDelegate : Delegate
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            throw new ArgumentException("Template cannot be empty", nameof(template));
        }

        ArgumentNullException.ThrowIfNull(handler);

        var segments = template.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var routeHandler = RouteHandler.Create(handler);

        var routeDef = new RouteDefinition
        {
            Template = template,
            Handler = routeHandler,
            Segments = [.. segments.Select(ParseSegmentDefinition)]
        };

        _lock.EnterWriteLock();
        try
        {
            _root.AddRoute(segments, 0, routeHandler);
            _routeRegistry[template] = routeDef;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public async Task<RouteResult> RouteAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            _lock.EnterReadLock();

            var match = _root.Match(path.Split('/', StringSplitOptions.RemoveEmptyEntries), 0, []);

            return match == null
                ? RouteResult.Fail($"No route found for path: {path}")
                : await ExecuteHandlerAsync(match.Handler, match.Parameters, cancellationToken);
        }
        finally
        {
            if (_lock.IsReadLockHeld)
            {
                _lock.ExitReadLock();
            }
        }
    }

    private static async Task<RouteResult> ExecuteHandlerAsync(RouteHandler handler, Dictionary<string, object> parameters, CancellationToken cancellationToken)
    {
        return await Task.Run(async () =>
        {
            try
            {
                var delegateParams = handler.HandlerDelegate.Method.GetParameters();
                var callArgs = new object?[delegateParams.Length];

                for (int i = 0; i < delegateParams.Length; i++)
                {
                    var paramInfo = delegateParams[i];
                    if (parameters.TryGetValue(paramInfo.Name!, out var value))
                    {
                        if (value != null && !paramInfo.ParameterType.IsAssignableFrom(value.GetType()))
                        {
                            value = Convert.ChangeType(value, paramInfo.ParameterType);
                        }

                        callArgs[i] = value;
                    }
                    else
                    {
                        callArgs[i] = paramInfo.DefaultValue;
                    }
                }

                if (cancellationToken != default)
                {
                    for (int i = 0; i < delegateParams.Length; i++)
                    {
                        if (delegateParams[i].ParameterType == typeof(CancellationToken))
                        {
                            callArgs[i] = cancellationToken;
                            break;
                        }
                    }
                }

                var result = handler.HandlerDelegate.DynamicInvoke(callArgs);

                if (handler.IsAsync)
                {
                    if (result is Task task)
                    {
                        await task.ConfigureAwait(false);

                        if (handler.ReturnType.IsGenericType)
                        {
                            var resultProperty = task.GetType().GetProperty("Result");
                            return RouteResult.Success(resultProperty?.GetValue(task));
                        }

                        return RouteResult.Success();
                    }
                }

                return RouteResult.Success(result);
            }
            catch (Exception ex)
            {
                return RouteResult.Fail($"Handler execution failed: {ex.Message}");
            }
        }, cancellationToken);
    }

    private static RouteSegmentDefinition ParseSegmentDefinition(string segment)
    {
        var def = new RouteSegmentDefinition { OriginalValue = segment };

        if (segment.StartsWith('{') && segment.EndsWith('}'))
        {
            def.IsDynamic = true;
            var content = segment.Trim('{', '}');
            var parts = content.Split(':');

            if (parts.Length == 2)
            {
                def.ParameterName = parts[0];
                def.ParameterType = parts[1];
            }
        }

        return def;
    }
}
