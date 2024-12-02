using System.Net;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Tests;

public class ToDoEndpointsTests(AppFactory factory) : IClassFixture<AppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetToDosWithinDateRange_ValidRange_ReturnsToDos()
    {
        // arrange
        var days = new[] { 23, 45 };
        await CreateTodo(_client, "DateRange test 1", "Range test 1 desc", days[0]);
        await CreateTodo(_client, "DateRange test 2", "Range test 2 desc", days[1]);
        
        // act
        var response = await _client.GetAsync($"/todos/upcoming/{days[1]}");
        
        // assert
        response.EnsureSuccessStatusCode();
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
        Assert.Contains(todos, t => t.Title == "DateRange test 1");
        Assert.Contains(todos, t => t.Title == "DateRange test 2");
    }
    
    [Fact]
    public async Task GetAllToDos_ShouldReturnOk_WhenThereAreTodos()
    {   
        // arrange
        await CreateTodo(_client, "ThereAreSomeTodosTest 1", "Test 1 desc");
        await CreateTodo(_client, "ThereAreSomeTodosTest 2", "Test 2 desc");
        
        // act
        var response = await _client.GetAsync("/todos");
        
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
        var nonExistingId = Guid.NewGuid();
        
        // act
        var response = await _client.GetAsync($"/todos/{nonExistingId}");
        
        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetToDoById_ExistingId_ReturnsToDo()
    {
        // arrange
        const string title = "Test";
        const string description = "Test desc";
        var id = await CreateTodo(_client, title, description);
        
        // act 
        var getResponse = await _client.GetAsync($"/todos/{id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedToDo = await getResponse.Content.ReadFromJsonAsync<ToDo>();
        
        // assert
        Assert.NotNull(fetchedToDo);
        Assert.Equal(title, fetchedToDo.Title);
        Assert.Equal(description, fetchedToDo.Description);
        Assert.Equal(id, fetchedToDo.Id);
    }

    [Fact]
    public async Task SearchToDosByTitle_ExistingTitle_ReturnsMatchingToDos()
    {
        // arrange
        const string title = "Search test";
        const string description = "Search test description";
        await CreateTodo(_client, title, description);
        
        // act
        var searchResponse = await _client.GetAsync($"/todos/search/{title}");
        searchResponse.EnsureSuccessStatusCode();
        var todos = await searchResponse.Content.ReadFromJsonAsync<List<ToDo>>();
        
        // assert
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
        Assert.All(todos, todo => Assert.Contains(title, todo.Title));

    }
    
    [Fact]
    public async Task SearchToDosByTitle_NonExistingTitle_ReturnsNotFound()
    {
        // arrange
        const string nonExistingTitle = "NonExistingTitle!@#$%^&*()_+-={}[]";

        // act
        var searchResponse = await _client.GetAsync($"/todos/search/{nonExistingTitle}");

        // assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, searchResponse.StatusCode);
    }    
    
    [Fact]
    public async Task SearchToDosByTitle_Null_ReturnsNotFound()
    {
        // arrange
        const string? nonExistingTitle = null;

        // act
        var searchResponse = await _client.GetAsync($"/todos/search/{nonExistingTitle}");

        // assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, searchResponse.StatusCode);
    }

    private static async Task<Guid> CreateTodo(HttpClient client, string title, string description, double days = 1)
    {
        var toDo = new ToDo
        {
            Title = title,
            Description = description,
            Expiry = DateTimeOffset.Now.AddDays((double)days!)
        };
        var creatingResponse = await client.PostAsJsonAsync("/todos", toDo);
        creatingResponse.EnsureSuccessStatusCode();

        var createdToDo = await creatingResponse.Content.ReadFromJsonAsync<ToDo>();
        return createdToDo!.Id;
    }



}