using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Infrastructure.Persistence;
using ToDoApp.Infrastructure.Repositories;

namespace ToDoApp.Tests.Persistence;

public class SubTaskPersistenceTests
{
    [Fact(DisplayName = "Persiste criação, reordenação e remoção do checklist")]
    public async Task GivenTaskChecklist_WhenMutatingAndReloading_ThenPersistsOrderedSubtasks()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new TodoDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var user = new User("Test User", "test@example.com", "hash");
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var repository = new TodoRepository(context);
        var task = new ToDo(user.Id, "Task", null, Priority.Low, null);
        await repository.AddAsync(task);

        var loadedTask = await repository.GetByIdWithSubtasksAsync(user.Id, task.Id);
        Assert.NotNull(loadedTask);
        var first = loadedTask.AddSubtask("First");
        await repository.AddSubtaskAsync(first);

        var second = loadedTask.AddSubtask("Second");
        await repository.AddSubtaskAsync(second);

        var third = loadedTask.AddSubtask("Third");
        await repository.AddSubtaskAsync(third);

        context.ChangeTracker.Clear();
        loadedTask = await repository.GetByIdWithSubtasksAsync(user.Id, task.Id);
        Assert.NotNull(loadedTask);
        Assert.Equal(3, loadedTask.Subtasks.Count);
        Assert.True(loadedTask.UpdateSubtask(second.Id, "Updated second", true));
        await repository.UpdateAsync(loadedTask);

        Assert.True(loadedTask.ReorderSubtasks([third.Id, first.Id, second.Id]));
        await repository.UpdateAsync(loadedTask);

        context.ChangeTracker.Clear();
        loadedTask = await repository.GetByIdWithSubtasksAsync(user.Id, task.Id);
        Assert.NotNull(loadedTask);
        Assert.Equal(
            [third.Id, first.Id, second.Id],
            loadedTask.Subtasks.OrderBy(subtask => subtask.Position).Select(subtask => subtask.Id));

        Assert.True(loadedTask.RemoveSubtask(first.Id));
        await repository.UpdateAsync(loadedTask);

        context.ChangeTracker.Clear();
        loadedTask = await repository.GetByIdWithSubtasksAsync(user.Id, task.Id);
        Assert.NotNull(loadedTask);
        Assert.Equal(
            [third.Id, second.Id],
            loadedTask.Subtasks.OrderBy(subtask => subtask.Position).Select(subtask => subtask.Id));
        Assert.Equal([0, 1], loadedTask.Subtasks.OrderBy(subtask => subtask.Position).Select(subtask => subtask.Position));

        await repository.DeleteAsync(user.Id, task.Id);
        Assert.Null(await repository.GetByIdWithSubtasksAsync(user.Id, task.Id));
        Assert.True(await repository.RestoreAsync(user.Id, task.Id));

        context.ChangeTracker.Clear();
        loadedTask = await repository.GetByIdWithSubtasksAsync(user.Id, task.Id);
        Assert.NotNull(loadedTask);
        var restoredSecond = Assert.Single(loadedTask.Subtasks, subtask => subtask.Id == second.Id);
        Assert.Equal("Updated second", restoredSecond.Title);
        Assert.True(restoredSecond.IsCompleted);
    }
}
