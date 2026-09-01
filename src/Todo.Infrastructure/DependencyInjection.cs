using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.Infrastructure.Repositories;

namespace ToDoApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=todo.db";

        services.AddDbContext<TodoDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IToDoRepository, TodoRepository>();

        return services;
    }
}