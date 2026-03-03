using RnD.DelegateRouter.Services;

namespace RouterTests;

public class Tests
{
    private readonly RouterService router = new();
    [Fact]
    public async Task BasicTest()
    {
        router.RegisterRoute("/foo/bar/", () => "Static route executed");
        var result = await router.RouteResultAsync("/foo/bar/");

        Assert.NotNull(result);
        Assert.Equal(result.IsSuccess, true);
        Assert.Equal(result.Result.ToString()!, "Static route executed");
    }

    [Fact]
    public async Task TestOneParam()
    {
        int parameter = 100;
        router.RegisterRoute("/foo/bar/{id:int}/", (int id) => $"Product {id} requested");
        var result = await router.RouteResultAsync($"/foo/bar/{parameter}");

        Assert.NotNull(result);
        Assert.Equal(result.IsSuccess, true);
        Assert.Equal(result.Result.ToString()!, $"Product {parameter} requested");
    }

    /*
    [Fact]
    public async Task BasicTest()
    {
        router.RegisterRoute("/foo/bar/", () => "Static route executed");
        var result = await router.RouteResultAsync("/foo/bar/");

        Assert.NotNull(result);
        Assert.Equal(result.IsSuccess, true);
        Assert.Equal(result.Result.ToString()!, "Static route executed");
    }

    [Fact]
    public async Task BasicTest()
    {
        router.RegisterRoute("/foo/bar/", () => "Static route executed");
        var result = await router.RouteResultAsync("/foo/bar/");

        Assert.NotNull(result);
        Assert.Equal(result.IsSuccess, true);
        Assert.Equal(result.Result.ToString()!, "Static route executed");
    }

    [Fact]
    public async Task BasicTest()
    {
        router.RegisterRoute("/foo/bar/", () => "Static route executed");
        var result = await router.RouteResultAsync("/foo/bar/");

        Assert.NotNull(result);
        Assert.Equal(result.IsSuccess, true);
        Assert.Equal(result.Result.ToString()!, "Static route executed");
    }

    [Fact]
    public async Task BasicTest()
    {
        router.RegisterRoute("/foo/bar/", () => "Static route executed");
        var result = await router.RouteResultAsync("/foo/bar/");

        Assert.NotNull(result);
        Assert.Equal(result.IsSuccess, true);
        Assert.Equal(result.Result.ToString()!, "Static route executed");
    }*/
}
