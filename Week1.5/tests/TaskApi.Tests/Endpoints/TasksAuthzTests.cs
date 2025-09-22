using System.Net;
using TaskApi.Tests;
using Xunit;

public class TasksAuthzTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public TasksAuthzTests(TestWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task List_Without_Token_Is_401()
    {
        var r = await _client.GetAsync("/api/tasks");
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }
}
