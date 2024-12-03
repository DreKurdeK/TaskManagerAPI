using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerAPI.Data;
using Testcontainers.PostgreSql;
using Npgsql;

namespace TaskManagerAPI.Tests
{
    public sealed class AppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _databaseContainer = new PostgreSqlBuilder()
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithDatabase("todoapp")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                RemoveDbContextConfiguration(services);

                services.AddDbContext<ToDoDbContext>(options =>
                {
                    options.UseNpgsql(_databaseContainer.GetConnectionString());
                });
            });
        }

        private static void RemoveDbContextConfiguration(IServiceCollection services)
        {
            var dbContextDescriptor = services
                .SingleOrDefault(descriptor => descriptor.ServiceType == typeof(DbContextOptions<ToDoDbContext>));
            
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var efCoreDescriptors = services
                .Where(descriptor => descriptor.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                .ToList();
            
            foreach (var descriptor in efCoreDescriptors)
                services.Remove(descriptor);
        }

        public async Task InitializeAsync()
        {
            await _databaseContainer.StartAsync();

            var connectionString = _databaseContainer.GetConnectionString();
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        public new async Task DisposeAsync()
        {
            await _databaseContainer.StopAsync();
        }
    }
}