using System.Net;
using System.Net.Http.Json;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Tests;

public class ToDoEndpointsTests(AppFactory factory) : IClassFixture<AppFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DeleteToDo_NonExistingId_ReturnsNotFound()
    {
        // arrange
        var nonExistingId = Guid.NewGuid();
        
        // act
        var deleteResponse = await _client.DeleteAsync($"/todos/{nonExistingId}");
        
        // assert
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeleteToDo_ExistingId_DeletesToDo()
    {
        // arrange
        var deletedTitle = "Deleted Title " + Guid.NewGuid();
        var deletedDescription = "Deleted Description " + Guid.NewGuid();
        var id = await CreateTodo(_client, deletedTitle, deletedDescription);
        
        // assert
        var initialGetResponse = await _client.GetAsync($"/todos/{id}");
        initialGetResponse.EnsureSuccessStatusCode();
        var initialFetchedToDo = await initialGetResponse.Content.ReadFromJsonAsync<ToDo>();
        Assert.NotNull(initialFetchedToDo);
        Assert.Equal(deletedTitle, initialFetchedToDo.Title);
        Assert.Equal(deletedDescription, initialFetchedToDo.Description);
        
        // act
        var deleteResponse = await _client.DeleteAsync($"/todos/{id}");
        deleteResponse.EnsureSuccessStatusCode();
        
        // assert
        var getResponse = await _client.GetAsync($"/todos/");
        getResponse.EnsureSuccessStatusCode();
        var fetchedToDos = await getResponse.Content.ReadFromJsonAsync<List<ToDo>>();
        
        // assert
        Assert.NotNull(fetchedToDos);
        Assert.DoesNotContain(fetchedToDos!, t => t.Title == deletedTitle && t.Description == deletedDescription);
        
    }
    
    [Fact]
    public async Task MarkToDoAsDone_NonExistingId_ReturnsNotFound()
    {
        //arrange 
        var nonExistingId = Guid.NewGuid();
        
        // act 
        var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"/todos/{nonExistingId}/done");
        var patchResponse = await _client.SendAsync(patchRequest); // PatchAsync is expecting a different set of parameters
        
        // arrange
        Assert.Equal(HttpStatusCode.NotFound, patchResponse.StatusCode);
    }
    
    [Fact]
    public async Task MarkToDoAsDone_ExistingId_MarksAsDone()
    {
        // arrange
        var expectedTitle = "MarkAsDone " + Guid.NewGuid();
        var expectedDescription = "MarkAsDoneExistingId " + Guid.NewGuid();
        var id = await CreateTodo(_client, expectedTitle, expectedDescription);
        
        var initialGetResponse = await _client.GetAsync($"/todos/{id}");
        initialGetResponse.EnsureSuccessStatusCode();
        var initialFetchedToDo = await initialGetResponse.Content.ReadFromJsonAsync<ToDo>();
        Assert.NotNull(initialFetchedToDo);
        Assert.False(initialFetchedToDo.IsDone);
        
        // act 
        var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"/todos/{id}/done");
        var patchResponse = await _client.SendAsync(patchRequest); // PatchAsync is expecting a different set of parameters
        patchResponse.EnsureSuccessStatusCode();
        
        var getResponse = await _client.GetAsync($"/todos/{id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedToDo = await getResponse.Content.ReadFromJsonAsync<ToDo>();
    
        // assert
        Assert.NotNull(fetchedToDo);
        Assert.Equal(expectedTitle, fetchedToDo.Title);
        Assert.Equal(expectedDescription, fetchedToDo.Description);
        Assert.Equal(id, fetchedToDo.Id);
        Assert.True(fetchedToDo.IsDone);
    }
    
    [Fact]
    public async Task UpdateToDo_NonExistingId_ReturnsNotFound()
    {
        // arrange
        var nonExistingId = Guid.NewGuid();
        var newTitle = "UpdateNewTitle " + Guid.NewGuid();
        var newDescription = "UpdateNewDescription " + Guid.NewGuid();
        var updatedToDo = new ToDo
        {
            Title = newTitle,
            Description = newDescription
        };
        
        // act 
        var updateResponse = await _client.PutAsJsonAsync($"/todos/{nonExistingId}", updatedToDo);
        
        // assert
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }
    
    [Fact]
    public async Task UpdateToDo_ExistingId_UpdatesToDo()
    {
        // arrange
        var expectedTitle = "UpdateTodoTitle " + Guid.NewGuid();
        var expectedDescription = "UpdateExistingTodoDesc " + Guid.NewGuid();
        var id = await CreateTodo(_client, expectedTitle, expectedDescription);
        
        var newTitle = "UpdateNewTitle " + Guid.NewGuid();
        var newDescription = "UpdateNewDescription " + Guid.NewGuid();
        var updatedToDo = new ToDo
        {
            Title = newTitle,
            Description = newDescription
        };
    
        // act 
        var updateResponse = await _client.PutAsJsonAsync($"/todos/{id}", updatedToDo);
        updateResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/todos/{id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedToDo = await getResponse.Content.ReadFromJsonAsync<ToDo>();
    
        // assert
        Assert.NotNull(fetchedToDo);
        Assert.Equal(id, fetchedToDo.Id);
        Assert.Equal(newTitle, fetchedToDo.Title);
        Assert.Equal(newDescription, fetchedToDo.Description);
    }   
    
    [Fact]
    public async Task CreateToDo_InvalidDate_ReturnsBadRequest()
    {   
        // arrange
        var invalidToDo = new ToDo 
        { 
            Title = "Test ToDo", 
            Description = "Test Description", 
            Expiry = DateTimeOffset.MinValue // invalid date
        };
    
        // act
        var response = await _client.PostAsJsonAsync("/todos", invalidToDo);
    
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateToDo_InvalidData_ReturnsBadRequest()
    {
        // arrange 
        var invalidTodo = new ToDo
        {
            Title = string.Empty,
            Description = "Invalid Todo",
            Expiry = DateTimeOffset.UtcNow.AddDays(-1)
        };
        
        // act 
        var response = await _client.PostAsJsonAsync("/todos", invalidTodo);
        
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateToDo_ValidData_ReturnsCreatedToDo()
    {
        // arrange
        var title = "NewTodo " + Guid.NewGuid();
        var description = "NewTodoDesc" + Guid.NewGuid();

        var newTodo = new ToDo
        {
            Title = title,
            Description = description,
            Expiry = DateTimeOffset.UtcNow.AddDays(3)
        };
        
        // act 
        var response = await _client.PostAsJsonAsync("/todos", newTodo);
        
        // assert
        response.EnsureSuccessStatusCode();

        var createdToDo = await response.Content.ReadFromJsonAsync<ToDo>();
        Assert.NotNull(createdToDo);
        Assert.Equal(title, createdToDo.Title);
        Assert.Equal(description, createdToDo.Description);
        Assert.Equal(newTodo.Expiry.Date, createdToDo.Expiry.Date);
        Assert.NotEqual(Guid.Empty, createdToDo.Id);
    }
    
    [Fact]
    public async Task GetToDosForCurrentWeek_NoToDosThisWeek_ReturnsNoToDo()
    {
        // arrange 
        var descNoTodos = "NoTodosThisWeek " + Guid.NewGuid();
        var titleNextWeek = "NextWeekTest " + Guid.NewGuid();
        
        await CreateTodo(_client, titleNextWeek, descNoTodos, 8); // +7 days for next week

        // act
        var response = await _client.GetAsync("/todos/current-week");

        // assert
        response.EnsureSuccessStatusCode();

        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        
        Assert.DoesNotContain(todos, t => t.Title == titleNextWeek);
    }

    [Fact]
    public async Task GetToDosForCurrentWeek_ValidDate_ReturnsToDos()
    {
        // arrange
        var descValidTodos = "ValidTodosThisWeek " + Guid.NewGuid();
        var titleForThisWeek = "ThisWeekTest " + Guid.NewGuid();
        var titleForNextWeek = "NextWeekTest " + Guid.NewGuid();

        //  the current week is from Monday to Sunday
        await CreateTodo(_client, titleForThisWeek, descValidTodos, 1); // This week (current week)
        await CreateTodo(_client, titleForNextWeek, descValidTodos, 7); // Next week

        // act
        var response = await _client.GetAsync("/todos/current-week");

        // assert
        response.EnsureSuccessStatusCode();

        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);

        Assert.Contains(todos, t => t.Title == titleForThisWeek);
        Assert.DoesNotContain(todos, t => t.Title == titleForNextWeek);
    }

    
    [Fact]
    public async Task GetToDosForTomorrow_NoToDosTomorrow_ReturnsNoToDo()
    {
        // arrange 
        var descNoTodos = "NoTodosTomorrow " + Guid.NewGuid();
        var titleToday = "TodayTest " + Guid.NewGuid();
        var titleDayAfter = "DayAfterTest " + Guid.NewGuid();
    
        await CreateTodo(_client, titleToday, descNoTodos, 0); // 1 for tomorrow
        await CreateTodo(_client, titleDayAfter, descNoTodos, 2); // 2 for the day after tomorrow 
    
        // act
        var response = await _client.GetAsync("/todos/next-day");
    
        // assert
        response.EnsureSuccessStatusCode();
    
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
    
        Assert.DoesNotContain(todos, t => t.Title == titleToday);
        Assert.DoesNotContain(todos, t => t.Title == titleDayAfter);
    }

    [Fact]
    public async Task GetToDosForTomorrow_ValidDate_ReturnsToDos()
    {
        // arrange
        var descValidTodos = "ValidTodosTomorrow " + Guid.NewGuid();
        var titleForToday = "TodayTest " + Guid.NewGuid();
        var titleForTomorrow = "TomorrowTest " + Guid.NewGuid();
        var titleForDayAfter = "DayAfterTest " + Guid.NewGuid();
    
        await CreateTodo(_client, titleForToday, descValidTodos, 0); // 0 for today
        await CreateTodo(_client, titleForTomorrow, descValidTodos, 1); // 1 for tomorrow
        await CreateTodo(_client, titleForDayAfter, descValidTodos, 2); // 2 for the day after tomorrow 
    
        // act
        var response = await _client.GetAsync("/todos/next-day");
    
        // assert
        response.EnsureSuccessStatusCode();
    
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);

        Assert.DoesNotContain(todos, t => t.Title == titleForToday);
        Assert.Contains(todos, t => t.Title == titleForTomorrow);
        Assert.DoesNotContain(todos, t => t.Title == titleForDayAfter);
    }
    
    [Fact]
    public async Task GetToDosForToday_NoToDosToday_ReturnsNoToDo()
    {
        // arrange 
        var descNoTodos = "NoTodosToday " + Guid.NewGuid().ToString();
        var titleForTomorrow = "TomorrowTest " + Guid.NewGuid();
        var titleForDayAfter = "DayAfterTest " + Guid.NewGuid();
    
        await CreateTodo(_client, titleForTomorrow, descNoTodos, 1); // 1 for tomorrow
        await CreateTodo(_client, titleForDayAfter, descNoTodos, 2); // 2 for the day after tomorrow 
    
        // act
        var response = await _client.GetAsync("/todos/today");
    
        // assert
        response.EnsureSuccessStatusCode();
    
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
    
        Assert.DoesNotContain(todos, t => t.Title == titleForTomorrow);
        Assert.DoesNotContain(todos, t => t.Title == titleForDayAfter);
    }

    [Fact]
    public async Task GetToDosForToday_ValidDate_ReturnsToDos()
    {
        // arrange
        var descValidTodos = "ValidTodosToday " + Guid.NewGuid().ToString();
        var titleForToday = "TodayTest " + Guid.NewGuid();
        var titleForTomorrow = "TomorrowTest " + Guid.NewGuid();
        var titleForDayAfter = "DayAfterTest " + Guid.NewGuid();
    
        await CreateTodo(_client, titleForToday, descValidTodos, 0); // 0 for today
        await CreateTodo(_client, titleForTomorrow, descValidTodos, 1); // 1 for tomorrow
        await CreateTodo(_client, titleForDayAfter, descValidTodos, 2); // 2 for the day after tomorrow 
    
        // act
        var response = await _client.GetAsync("/todos/today");
    
        // assert
        response.EnsureSuccessStatusCode();
    
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);

        Assert.Contains(todos, t => t.Title == titleForToday);
        Assert.DoesNotContain(todos, t => t.Title == titleForTomorrow);
        Assert.DoesNotContain(todos, t => t.Title == titleForDayAfter);
    }

    
    [Fact]
    public async Task GetToDosWithinDateRange_NoToDosInRange_ReturnsEmptyList()
    {
        // arrange
        const int days = -1;
        
        // act
        var response = await _client.GetAsync($"/todos/upcoming/{days}"); // should return nothing because you can only create todos in future
        
        // assert
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.Empty(todos!);
    }
    
    [Fact]
    public async Task GetToDosWithinDateRange_ValidRange_ReturnsToDos()
    {
        // arrange
        var days = new[] { 23, 45, 60 };
        var titleDay23 = "Day23 " + Guid.NewGuid();
        var titleDay45 = "Day45 " + Guid.NewGuid();
        var titleDay60 = "Day60 " + Guid.NewGuid();
        var description = "RangeTestDesc" + Guid.NewGuid();
        await CreateTodo(_client, titleDay23, description, days[0]);
        await CreateTodo(_client, titleDay45, description, days[1]);
        await CreateTodo(_client, titleDay60, description, days[2]);
        
        // act
        var response = await _client.GetAsync($"/todos/upcoming/{days[1]}");
        
        // assert
        response.EnsureSuccessStatusCode();
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
        Assert.Contains(todos, t => t.Title == titleDay23);
        Assert.Contains(todos, t => t.Title == titleDay45);
        Assert.DoesNotContain(todos, t => t.Title == titleDay60);
    }

    [Fact]
    public async Task GetAllToDos_ShouldReturnOk_WhenThereAreTodos()
    {   
        // arrange
        var description = "AllTodosDesc " + Guid.NewGuid();
        var titleTodo1 = "Todo1" + Guid.NewGuid();
        var titleTodo2 = "Todo2" + Guid.NewGuid();
        
        await CreateTodo(_client, titleTodo1, description);
        await CreateTodo(_client, titleTodo2, description);
        
        // act
        var response = await _client.GetAsync("/todos");
        
        // assert
        response.EnsureSuccessStatusCode();
        var todos = (await response.Content.ReadFromJsonAsync<List<ToDo>>())?.ToArray();
        Assert.NotNull(todos);
        Assert.Contains(todos, t => t.Title == titleTodo1);
        Assert.Contains(todos, t => t.Title == titleTodo2);
    }

    [Fact]
    public async Task GetToDoById_EmptyId_ReturnsBadRequest()
    {   
        // arrange
        var emptyId = Guid.Empty;
    
        // act
        var response = await _client.GetAsync($"/todos/{emptyId}");
    
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
        var expectedTitle = "ExistingTodo " + Guid.NewGuid();
        var expectedDescription = "ExistingTodoDesc " + Guid.NewGuid();
        var id = await CreateTodo(_client, expectedTitle, expectedDescription);
        
        // act 
        var getResponse = await _client.GetAsync($"/todos/{id}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedToDo = await getResponse.Content.ReadFromJsonAsync<ToDo>();
        
        // assert
        Assert.NotNull(fetchedToDo);
        Assert.Equal(expectedTitle, fetchedToDo.Title);
        Assert.Equal(expectedDescription, fetchedToDo.Description);
        Assert.Equal(id, fetchedToDo.Id);
    }

    [Fact]
    public async Task SearchToDosByTitle_ExistingTitle_ReturnsMatchingToDos()
    {
        // arrange
        var searchTitle = "SearchMatch " + Guid.NewGuid();
        var searchDescription = "SearchDesc " + Guid.NewGuid();
        await CreateTodo(_client, searchTitle, searchDescription);
        
        // act
        var searchResponse = await _client.GetAsync($"/todos/search/{searchTitle}");
        searchResponse.EnsureSuccessStatusCode();
        var todos = await searchResponse.Content.ReadFromJsonAsync<List<ToDo>>();
        
        // assert
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
        Assert.All(todos, todo => Assert.Contains(searchTitle, todo.Title));
    }

    [Fact]
    public async Task SearchToDosByTitle_NonExistingTitle_ReturnsNotFound()
    {
        // arrange
        var nonExistingTitle = "NoMatch" + Guid.NewGuid();

        // act
        var searchResponse = await _client.GetAsync($"/todos/search/{nonExistingTitle}");

        // assert
        Assert.Equal(HttpStatusCode.NotFound, searchResponse.StatusCode);
    }


    private static async Task<Guid> CreateTodo(HttpClient client, string title, string description, int days = 1)
    {
        var toDo = new ToDo
        {
            Title = title,
            Description = description,
            Expiry = DateTimeOffset.UtcNow.AddDays(days)
        };
        var creatingResponse = await client.PostAsJsonAsync("/todos", toDo);
        creatingResponse.EnsureSuccessStatusCode();

        var createdToDo = await creatingResponse.Content.ReadFromJsonAsync<ToDo>();
        return createdToDo!.Id;
    }
}