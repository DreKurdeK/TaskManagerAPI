using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskManagerAPI.Tests;

public class ToDoEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAllToDos_ShouldReturnOk_WhenThereAreTodos()
    {   
        var response = await _client.GetAsync("/todos");
        
        response.EnsureSuccessStatusCode();
        if (response.Content.Headers.ContentType != null)
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
    }

    [Fact]
    public async Task GetAllToDos_ShouldReturnEmptyList_WhenNoTodosExist()
    {
        var response = await _client.GetAsync("/todos");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        Assert.Equal("[]", content);
    }
}