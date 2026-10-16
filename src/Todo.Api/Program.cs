using ToDoApp.Infrastructure;
using ToDoApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.UseCases;
using ToDoApp.Infrastructure.Repositories;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Application.Validators;
using FluentValidation;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("A configuração Jwt:Key não foi definida.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["todo_auth"];
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/problem+json";
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Autenticação necessária",
                    Detail = "Faça login para acessar este recurso.",
                    Instance = context.Request.Path
                };
                problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                await context.Response.WriteAsJsonAsync(problemDetails);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/problem+json";
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Acesso negado",
                    Detail = "Você não tem permissão para acessar este recurso.",
                    Instance = context.Request.Path
                };
                problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        };
    });
builder.Services.AddAuthorization();

const string FrontendCorsPolicy = "FrontendCorsPolicy";
var frontendOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddOpenApi();
builder.Services
    .AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter());
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ToDoApp.Application.OptionalJsonConverterFactory());
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new ToDoApp.Application.OptionalJsonConverterFactory());
});
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
builder.Services.AddScoped<IToDoRepository, TodoRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CreateTaskUseCase>();
builder.Services.AddScoped<ListSubTasksUseCase>();
builder.Services.AddScoped<GetSubTaskUseCase>();
builder.Services.AddScoped<CreateSubTaskUseCase>();
builder.Services.AddScoped<UpdateSubTaskUseCase>();
builder.Services.AddScoped<DeleteSubTaskUseCase>();
builder.Services.AddScoped<ReorderSubTasksUseCase>();
builder.Services.AddScoped<ListTasksUseCase>();
builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<ListCategoriesUseCase>();
builder.Services.AddScoped<UpdateTaskUseCase>();
builder.Services.AddScoped<CompleteTaskUseCase>();
builder.Services.AddScoped<DeleteTaskUseCase>();
builder.Services.AddScoped<GetTrashUseCase>();
builder.Services.AddScoped<GetTrashCategoryUseCase>();
builder.Services.AddScoped<UpdateCategoryUseCase>();
builder.Services.AddScoped<RestoreTaskUseCase>();
builder.Services.AddScoped<RestoreCategoryUseCase>();
builder.Services.AddScoped<DeleteCategoryUseCase>();
builder.Services.AddScoped<GetSummaryUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<LoginUserUseCase>();


var app = builder.Build();

// Auto-create SQLite database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
app.UseMiddleware<ToDoApp.Api.Middlewares.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapPrometheusScrapingEndpoint();


app.Run();