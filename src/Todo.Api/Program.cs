using ToDoApp.Infrastructure;
using ToDoApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.UseCases;
using ToDoApp.Infrastructure.Repositories;
using ToDoApp.Domain.Interfaces.Repositories;
using ToDoApp.Application.Validators;
using FluentValidation;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
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

app.MapControllers();


app.Run();