namespace RnD.DelegateRouter.Entities;

public class RouteHandler
{
    public Delegate HandlerDelegate { get; }
    public Type[] ParameterTypes { get; }
    public Type ReturnType { get; }
    public bool IsAsync { get; }

    public RouteHandler(Delegate handlerDelegate, Type[] parameterTypes)
    {
        HandlerDelegate = handlerDelegate ?? throw new ArgumentNullException(nameof(handlerDelegate));
        ParameterTypes = parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes));

        var methodInfo = handlerDelegate.Method;
        ReturnType = methodInfo.ReturnType;
        IsAsync = typeof(Task).IsAssignableFrom(ReturnType);
    }

    public static RouteHandler Create<TDelegate>(TDelegate handler) where TDelegate : Delegate
    {
        var parameterTypes = handler.Method.GetParameters().Select(p => p.ParameterType).ToArray();
        return new RouteHandler(handler, parameterTypes);
    }

    public bool ValidateParameters(Dictionary<string, object> parameters)
    {
        var methodParams = HandlerDelegate.Method.GetParameters();

        foreach (var param in methodParams)
        {
            if (parameters.TryGetValue(param.Name!, out var value))
            {
                if (value != null && !param.ParameterType.IsAssignableFrom(value.GetType()))
                {
                    return false;
                }
            }
            else if (!param.IsOptional)
            {
                return false;
            }
        }

        return true;
    }

    public string GetParametersInfo()
    {
        var methodParams = HandlerDelegate.Method.GetParameters();
        return string.Join(", ", methodParams.Select(p => $"{p.ParameterType.Name} {p.Name}"));
    }
}
