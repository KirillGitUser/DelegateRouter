using RnD.DelegateRouter.Services;

namespace RouterTests;

public class Tests
{
    private readonly RouterService router = new();
    [Fact]
    public async Task BasicTest()
    {
        router.RegisterRoute("/foo/bar/", () => "Static route executed");
        var result = await router.RouteAsync("/foo/bar/");

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Result.ToString()!, "Static route executed");
    }

    [Fact]
    public async Task TestOneParam()
    {
        int parameter = 100;
        router.RegisterRoute("/foo/bar/{id:int}/", (int id) => $"Product {id} requested");
        var result = await router.RouteAsync($"/foo/bar/{parameter}");

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Result.ToString()!, $"Product {parameter} requested");
    }

    [Fact]
    public async Task TestCombinationOfParams()
    {
        router.RegisterRoute("/calculate/{a:int}/{b:int}/{operation:string}/",
            (string operation, int b, int a) =>
            {
                return operation switch
                {
                    "add" => a + b,
                    "sub" => a - b,
                    "mul" => a * b,
                    "div" => b != 0 ? a / b : 0,
                    _ => 0
                };
            });

        var result = await router.RouteAsync("/calculate/10/21/mul/");
        Assert.NotNull(result);
        Assert.Equal(result.Result, 210);

        result = await router.RouteAsync("/calculate/-50/1/add/");
        Assert.NotNull(result);
        Assert.Equal(result.Result, -49);

        result = await router.RouteAsync("/calculate/25855/369556/div/");
        Assert.NotNull(result);
        Assert.Equal(result.Result, 0);
    }

    [Fact]
    public async Task TestTask()
    {
        router.RegisterRoute("/users/{userId:guid}/posts/{postId:int}/",
        (Guid userId, int postId) =>
        {
            return Task.FromResult($"User {userId} post {postId}");
        });

        var result = await router.RouteAsync("/users/123e4567-e89b-12d3-a456-426614174000/posts/100/");

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Result, "User 123e4567-e89b-12d3-a456-426614174000 post 100");
    }
}
