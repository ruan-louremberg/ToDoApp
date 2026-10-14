using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.Infrastructure.Repositories;
using ToDoApp.Application.Interfaces;
using ToDoApp.Infrastructure.Services;
using Resend;

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
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISummaryRepository, SummaryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, ResendEmailService>();
        services.AddHttpClient<ResendClient>();
        services.AddTransient<IResend>(provider => provider.GetRequiredService<ResendClient>());
        services.Configure<ResendClientOptions>(options =>
        {
            options.ApiToken = configuration["Resend:ApiKey"]
                ?? Environment.GetEnvironmentVariable("RESEND_APIKEY")
                ?? Environment.GetEnvironmentVariable("RESEND_API_KEY")
                ?? string.Empty;
        });

        return services;
    }
}