using System.Net;
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
        var todos = await response.Content.ReadFromJsonAsync<List<ToDo>>();
        Assert.NotNull(todos);
        Assert.Empty(todos);
    }
    
    [Fact]
    public async Task GetAllToDos_ShouldReturnOk_WhenThereAreTodos()
    {   
        // arrange
        var client = factory.CreateClient();
        await CreateTodo(client, "ThereAreSomeTodosTest 1", "Test 1 desc");
        await CreateTodo(client, "ThereAreSomeTodosTest 2", "Test 2 desc");
        
        // act
        var response = await client.GetAsync("/todos");
        
        // assert
        response.EnsureSuccessStatusCode();
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        Assert.Contains(todos, t => t.Title == "ThereAreSomeTodosTest 1");
        Assert.Contains(todos, t => t.Title == "ThereAreSomeTodosTest 2");
    }    
    
    [Fact]
    public async Task GetToDoById_NonExistingId_ReturnsNotFound()
    {   
        // arrange
        var client = factory.CreateClient();
        var nonExistingId = Guid.NewGuid();
        
        // act
        var response = await client.GetAsync($"/todos/{nonExistingId}");
        
        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetToDoById_ExistingId_ReturnsToDo()
    {
        // arrange
        var client = factory.CreateClient();
        const string title = "Test";
        const string description = "Test desc";
        var id = await CreateTodo(client, title, description);
        
        // act 
        var getResponse = await client.GetAsync($"/todos/{id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedToDo = await getResponse.Content.ReadFromJsonAsync<ToDo>();
        
        // assert
        Assert.NotNull(fetchedToDo);
        Assert.Equal(title, fetchedToDo!.Title);
        Assert.Equal(description, fetchedToDo!.Description);
        Assert.Equal(id, fetchedToDo.Id);
    }

    private static async Task<Guid> CreateTodo(HttpClient client, string title, string description)
    {
        var toDo = new ToDo
        {
            Title = title,
            Description = description,
            Expiry = DateTimeOffset.Now.AddDays(7)
        };
        var creatingResponse = await client.PostAsJsonAsync("/todos", toDo);
        creatingResponse.EnsureSuccessStatusCode();

        var createdToDo = await creatingResponse.Content.ReadFromJsonAsync<ToDo>();
        return createdToDo!.Id;
    }



}