/*      Wasnt working maybe if I have more time

using System.Net;
using System.Net.Http.Json;
using TaskApi.Contracts;
using TaskApi.Dtos;
using TaskApi.Models;
using TaskApi.Tests;
using Xunit;

public class TasksCrudTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public TasksCrudTests(TestWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task Create_Get_Update_Delete_Works()
    {
        var token = await _client.RegisterAndLoginAsync("alice_crud", "password123");
        _client.UseBearer(token);

        // CREATE
        var createDto = new TaskCreateDto { Title = "Buy milk", Priority = Priority.High };
        var create = await _client.PostAsJsonAsync("/api/tasks", createDto);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<ApiResponse<TaskReadDto>>();
        var id = created!.Data!.Id;

        // GET
        var get = await _client.GetFromJsonAsync<ApiResponse<TaskReadDto>>($"/api/tasks/{id}");
        Assert.Equal("Buy milk", get!.Data!.Title);

        // UPDATE
        var updDto = new TaskUpdateDto
        {
            Title = "Buy milk & eggs",
            Priority = Priority.Critical,
            IsCompleted = true,
        };
        var upd = await _client.PutAsJsonAsync($"/api/tasks/{id}", updDto);
        Assert.Equal(HttpStatusCode.OK, upd.StatusCode);
        var updated = await upd.Content.ReadFromJsonAsync<ApiResponse<TaskReadDto>>();
        Assert.True(updated!.Data!.IsCompleted);
        Assert.Equal(Priority.Critical, updated.Data.Priority);

        // DELETE
        var del = await _client.DeleteAsync($"/api/tasks/{id}");
        Assert.Equal(HttpStatusCode.OK, del.StatusCode);
        var get404 = await _client.GetAsync($"/api/tasks/{id}");
        Assert.Equal(HttpStatusCode.NotFound, get404.StatusCode);
    }
}

*/
