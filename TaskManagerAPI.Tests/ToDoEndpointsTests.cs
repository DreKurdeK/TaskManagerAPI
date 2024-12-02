using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Tests;

public class ToDoEndpointsTests(AppFactory factory) : IClassFixture<AppFactory>
{
    [Fact]
    public async Task GetAllToDos_ShouldReturnEmptyList_WhenNoTodosExist()
    {
        // arrange
        var client = factory.CreateClient(); 
            
        // act
        var response = await client.GetAsync("/todos");
        
        // assert
        response.EnsureSuccessStatusCode();
        var todos = await response.Content.ReadFromJsonAsync<IEnumerable<ToDo>>();
        Assert.NotNull(todos);
        Assert.Empty(todos);
    }
    
    [Fact]
    public async Task GetAllToDos_ShouldReturnOk_WhenThereAreTodos()
    {   
        // arrange
        var client = factory.CreateClient();
        await CreateTodo(client, "Test 1", "Test 1 desc");
        await CreateTodo(client, "Test 2", "Test 2 desc");
        
        // act
        var response = await client.GetAsync("/todos");
        
        // assert
        response.EnsureSuccessStatusCode();
        var todos = (await response.Content.ReadFromJsonAsync<IEnumerable<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        Assert.Equal(2, todos.Length);
        Assert.Contains(todos, t => t.Title == "Test 1");
        Assert.Contains(todos, t => t.Title == "Test 2");
    }

    private static async Task CreateTodo(HttpClient client, string title, string description)
    {
        var toDo = new ToDo
        {
            Title = title,
            Description = description,
            Expiry = DateTimeOffset.Now.AddDays(7)
        };
        var creatingResponse = await client.PostAsJsonAsync("/todos", toDo);
        creatingResponse.EnsureSuccessStatusCode();
    }


}