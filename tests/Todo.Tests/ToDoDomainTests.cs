using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Tests;

public class ToDoDomainTests
{
    [Fact]
    public void ChangeStatus_ToCompleted_ShouldSetCompletedAt()
    {
        var task = new ToDo("Estudar testes", null, default, null);

        task.ChangeStatus(Status.Completed);

        Assert.Equal(Status.Completed, task.Status);
        Assert.NotNull(task.CompletedAt);
        Assert.Equal(DateTimeKind.Utc, task.CompletedAt!.Value.Kind);
    }

    [Fact]
    public void ChangeStatus_ToPending_ShouldClearCompletedAt()
    {
        var task = new ToDo("Estudar testes", null, default, null);

        task.ChangeStatus(Status.Completed);
        task.ChangeStatus(Status.Pending);

        Assert.Equal(Status.Pending, task.Status);
        Assert.Null(task.CompletedAt);
    }
}