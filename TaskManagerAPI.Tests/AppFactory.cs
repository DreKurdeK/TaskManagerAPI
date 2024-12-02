using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerAPI.Data;

namespace TaskManagerAPI.Tests;

public sealed class AppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveExistingDbContext(services);
            services.AddDbContext<ToDoDbContext>(options =>
                options.UseInMemoryDatabase("todoapp"));
        });
    }

    private static void RemoveExistingDbContext(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(serviceDescriptor =>
            serviceDescriptor.ServiceType == typeof(DbContextOptions<ToDoDbContext>));
        
        if (descriptor != null)
            services.Remove(descriptor);

        var providerDescriptors = services
            .Where(serviceDescriptor => serviceDescriptor
                .ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
            .ToList();
        
        foreach (var providerDescriptor in providerDescriptors)
            services.Remove(providerDescriptor);
    }
}
