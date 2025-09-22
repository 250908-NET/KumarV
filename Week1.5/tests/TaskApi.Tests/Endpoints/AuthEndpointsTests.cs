using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskApi.Contracts;
using TaskApi.Dtos;
using TaskApi.Tests;
using Xunit;

namespace TaskApi.Tests.Endpoints;

public class AuthEndpointsTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(TestWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task Register_Then_Login_Returns_Token()
    {
        var reg = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterDto { UserName = "alice", Password = "password123" }
        );
        reg.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto { UserName = "alice", Password = "password123" }
        );
        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await login.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_Wrong_Password_Returns_401()
    {
        await _client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterDto { UserName = "bob", Password = "password123" }
        );

        var login = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto { UserName = "bob", Password = "nope" }
        );

        login.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
