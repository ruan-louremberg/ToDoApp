using Moq;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class SendDueDateRemindersUseCaseTests
{
    [Fact]
    public async Task GivenMultipleDueTasksForOneUser_WhenSendingReminders_ThenSendsOneEscapedDigestAndMarksTasks()
    {
        var user = new User("<Maria>", "maria@example.com", "password-hash");
        var firstTask = CreateTask(user, "<script>alert(1)</script>");
        var secondTask = CreateTask(user, "Entregar relatório");
        var repository = new Mock<IToDoRepository>();
        repository
            .Setup(item => item.GetPendingTasksDueBetweenAsync(
                new DateTime(2026, 10, 14),
                new DateTime(2026, 10, 15),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([firstTask, secondTask]);
        var emailService = new Mock<IEmailService>();
        string? sentSubject = null;
        string? sentHtml = null;
        emailService
            .Setup(item => item.SendEmailAsync(
                user.Email,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, string, string, CancellationToken>((_, subject, html, _) =>
            {
                sentSubject = subject;
                sentHtml = html;
            })
            .Returns(Task.CompletedTask);

        var useCase = new SendDueDateRemindersUseCase(
            repository.Object,
            emailService.Object,
            "https://todo.example.com");

        var emailsSent = await useCase.ExecuteAsync(new DateTime(2026, 10, 14));

        Assert.Equal(1, emailsSent);
        Assert.Contains("2 tarefas", sentSubject);
        Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt;", sentHtml);
        Assert.Contains("&lt;Maria&gt;", sentHtml);
        Assert.Contains("todo.example.com", sentHtml);
        Assert.NotNull(firstTask.ReminderSentAt);
        Assert.NotNull(secondTask.ReminderSentAt);
        repository.Verify(item => item.UpdateAsync(firstTask, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(item => item.UpdateAsync(secondTask, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GivenEmailDeliveryFailure_WhenSendingReminders_ThenLeavesTasksUnmarkedForRetry()
    {
        var user = new User("Maria", "maria@example.com", "password-hash");
        var task = CreateTask(user, "Entregar relatório");
        var repository = new Mock<IToDoRepository>();
        repository
            .Setup(item => item.GetPendingTasksDueBetweenAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([task]);
        var emailService = new Mock<IEmailService>();
        emailService
            .Setup(item => item.SendEmailAsync(
                user.Email,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new EmailDeliveryException("Mail service unavailable."));

        var useCase = new SendDueDateRemindersUseCase(
            repository.Object,
            emailService.Object,
            "https://todo.example.com");

        await Assert.ThrowsAsync<EmailDeliveryException>(
            () => useCase.ExecuteAsync(DateTime.Today));

        Assert.Null(task.ReminderSentAt);
        repository.Verify(
            item => item.UpdateAsync(It.IsAny<ToDo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static ToDo CreateTask(User user, string title)
    {
        var task = new ToDo(user.Id, title, null, Priority.Medium, new DateTime(2026, 10, 14));
        typeof(ToDo).GetProperty(nameof(ToDo.User))!.SetValue(task, user);
        return task;
    }
}
