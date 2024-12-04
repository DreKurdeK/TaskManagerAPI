using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using TaskManagerAPI.DAL;

namespace TaskManagerAPI.Tests;

public sealed class AppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder()
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithDatabase("todoapp")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder
        .ConfigureAppConfiguration(configurationBuilder => configurationBuilder
            .AddInMemoryCollection([
                new(
                    key: "ConnectionStrings:DefaultConnection",
                    _databaseContainer.GetConnectionString())
            ]));

    public async Task InitializeAsync()
    {
        await _databaseContainer.StartAsync();
        await MigrateDb();
    }

    public new async Task DisposeAsync() => 
        await _databaseContainer.StopAsync();

    private async Task MigrateDb()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
        if ((await db.Database.GetPendingMigrationsAsync()).Any())
            await db.Database.MigrateAsync();
    }
}