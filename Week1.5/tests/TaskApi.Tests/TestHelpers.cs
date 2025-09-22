using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskApi.Contracts;
using TaskApi.Dtos;

namespace TaskApi.Tests;

public static class TestHelpers
{
    public static async Task<string> RegisterAndLoginAsync(
        this HttpClient client,
        string user,
        string pass
    )
    {
        await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterDto { UserName = user, Password = pass }
        );
        var resp = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto { UserName = user, Password = pass }
        );
        var body = await resp.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        return body!.Data!.Token!;
    }

    public static void UseBearer(this HttpClient client, string jwt) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
}
