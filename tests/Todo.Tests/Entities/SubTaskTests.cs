using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Tests.Entities;

public class SubTaskTests
{
    [Fact(DisplayName = "Adiciona subtarefas na ordem sequencial do checklist")]
    public void GivenTask_WhenAddingSubtasks_ThenAssignsSequentialPositions()
    {
        var task = CreateTask();

        var first = task.AddSubtask("First");
        var second = task.AddSubtask("Second");

        Assert.Equal(0, first.Position);
        Assert.Equal(1, second.Position);
        Assert.Equal(task.Id, first.TaskId);
        Assert.Contains(first, task.Subtasks);
        Assert.NotNull(task.UpdatedAt);
    }

    [Fact(DisplayName = "Atualiza conclusão e título da subtarefa")]
    public void GivenExistingSubtask_WhenUpdating_ThenChangesRequestedValues()
    {
        var task = CreateTask();
        var subtask = task.AddSubtask("First");

        var updated = task.UpdateSubtask(subtask.Id, "Renamed", true);

        Assert.True(updated);
        Assert.Equal("Renamed", subtask.Title);
        Assert.True(subtask.IsCompleted);
        Assert.NotNull(subtask.UpdatedAt);
    }

    [Fact(DisplayName = "Remove subtarefa e compacta as posições restantes")]
    public void GivenSeveralSubtasks_WhenRemovingOne_ThenCompactsPositions()
    {
        var task = CreateTask();
        var first = task.AddSubtask("First");
        var second = task.AddSubtask("Second");
        var third = task.AddSubtask("Third");

        var removed = task.RemoveSubtask(second.Id);

        Assert.True(removed);
        Assert.Equal([first, third], task.Subtasks.OrderBy(item => item.Position));
        Assert.Equal([0, 1], task.Subtasks.OrderBy(item => item.Position).Select(item => item.Position));
    }

    [Fact(DisplayName = "Reordena todas as subtarefas por identificador")]
    public void GivenCompleteOrder_WhenReordering_ThenUpdatesPositions()
    {
        var task = CreateTask();
        var first = task.AddSubtask("First");
        var second = task.AddSubtask("Second");

        var reordered = task.ReorderSubtasks([second.Id, first.Id]);

        Assert.True(reordered);
        Assert.Equal(0, second.Position);
        Assert.Equal(1, first.Position);
    }

    [Fact(DisplayName = "Rejeita uma ordem que omite subtarefas sem alterar posições")]
    public void GivenIncompleteOrder_WhenReordering_ThenDoesNotChangePositions()
    {
        var task = CreateTask();
        var first = task.AddSubtask("First");
        task.AddSubtask("Second");

        var reordered = task.ReorderSubtasks([first.Id]);

        Assert.False(reordered);
        Assert.Equal(0, first.Position);
    }

    private static ToDo CreateTask() => new("Task", null, Priority.Low, null);
}
