using ToDoApp.Domain;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Tests.Entities;

public class ToDoTests
{
    [Fact(DisplayName = "Cria tarefa pendente com os dados informados")]
    public void GivenTaskData_WhenConstructed_ThenInitializesPendingTask()
    {
        var categoryId = Guid.NewGuid();
        var task = new ToDo("Task", "Description", Priority.High, DateTime.UtcNow.AddDays(1), categoryId);

        Assert.Equal("Task", task.Title);
        Assert.Equal(Status.Pending, task.Status);
        Assert.Equal(Priority.High, task.Priority);
        Assert.Equal(categoryId, task.CategoryId);
        Assert.Null(task.CompletedAt);
        Assert.False(task.IsDeleted);
    }

    [Fact(DisplayName = "Associa categoria e atualiza o identificador")]
    public void GivenTaskAndCategory_WhenSettingCategory_ThenAssociatesCategory()
    {
        var task = new ToDo("Task", null, Priority.Low, null);
        var category = new Category("Work", "#FFFFFF");

        task.SetCategory(category);

        Assert.Same(category, task.Category);
        Assert.Equal(category.Id, task.CategoryId);
    }

    [Fact(DisplayName = "Atualiza apenas os campos fornecidos")]
    public void GivenExistingTask_WhenUpdatingWithPartialData_ThenPreservesMissingValues()
    {
        var dueDate = DateTime.UtcNow.AddDays(1);
        var task = new ToDo("Original", "Description", Priority.Low, dueDate);

        task.SetUpdate("Updated", Optional<string?>.Unset(), Priority.High, Optional<DateTime?>.Unset(), Optional<Guid?>.Unset());

        Assert.Equal("Updated", task.Title);
        Assert.Equal("Description", task.Description);
        Assert.Equal(Priority.High, task.Priority);
        Assert.Equal(dueDate, task.DueDate);
        Assert.NotNull(task.UpdatedAt);
    }

    [Fact(DisplayName = "Marca tarefa como concluída com data de conclusão")]
    public void GivenPendingTask_WhenChangingStatusToCompleted_ThenSetsCompletionDate()
    {
        var task = new ToDo("Task", null, Priority.Low, null);
        task.ChangeStatus(Status.InProgress);

        task.ChangeStatus(Status.Completed);

        Assert.Equal(Status.Completed, task.Status);
        Assert.NotNull(task.CompletedAt);
        Assert.NotNull(task.UpdatedAt);
    }

    [Fact(DisplayName = "Remove data de conclusão ao reabrir tarefa em progresso")]
    public void GivenCompletedTask_WhenChangingStatusToInProgress_ThenClearsCompletionDate()
    {
        var task = new ToDo("Task", null, Priority.Low, null);
        task.ChangeStatus(Status.InProgress);
        task.ChangeStatus(Status.Completed);

        task.ChangeStatus(Status.InProgress);

        Assert.Equal(Status.InProgress, task.Status);
        Assert.Null(task.CompletedAt);
    }

    [Fact(DisplayName = "Permite retornar tarefa em progresso para pendente")]
    public void GivenInProgressTask_WhenChangingStatusToPending_ThenChangesStatus()
    {
        var task = new ToDo("Task", null, Priority.Low, null);
        task.ChangeStatus(Status.InProgress);

        task.ChangeStatus(Status.Pending);

        Assert.Equal(Status.Pending, task.Status);
        Assert.Null(task.CompletedAt);
    }

    [Theory]
    [InlineData(Status.Pending, Status.Completed)]
    [InlineData(Status.Completed, Status.Pending)]
    public void GivenDisallowedTransition_WhenChangingStatus_ThenThrowsDomainException(
        Status initialStatus,
        Status newStatus)
    {
        var task = new ToDo("Task", null, Priority.Low, null);
        if (initialStatus != Status.Pending)
        {
            task.ChangeStatus(Status.InProgress);
            task.ChangeStatus(initialStatus);
        }

        Assert.Throws<Domain.Exceptions.DomainException>(() => task.ChangeStatus(newStatus));
    }

    [Fact(DisplayName = "Mantém data ao concluir tarefa já concluída")]
    public void GivenCompletedTask_WhenCompletingAgain_ThenKeepsExistingCompletionDate()
    {
        var task = new ToDo("Task", null, Priority.Low, null);
        task.ChangeStatus(Status.InProgress);
        task.ChangeStatus(Status.Completed);
        var completedAt = task.CompletedAt;

        task.ChangeStatus(Status.Completed);

        Assert.Equal(completedAt, task.CompletedAt);
    }

    [Fact(DisplayName = "Marca e restaura tarefa excluída logicamente")]
    public void GivenTask_WhenSoftDeletedAndRestored_ThenTogglesDeletionState()
    {
        var task = new ToDo("Task", null, Priority.Low, null);

        task.SoftDelete();
        Assert.True(task.IsDeleted);
        Assert.NotNull(task.DeletedAt);

        task.Restore();
        Assert.False(task.IsDeleted);
        Assert.Null(task.DeletedAt);
    }
}