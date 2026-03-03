namespace RnD.DelegateRouter.Entities;

public class RouteResult
{
    public bool IsSuccess { get; set; }
    public object? Result { get; set; }
    public string? ErrorMessage { get; set; }

    public static RouteResult Success(object? result = null) => new() { IsSuccess = true, Result = result };
    public static RouteResult Fail(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
