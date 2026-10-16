namespace ToDoApp.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);

    Task SendPasswordResetEmailAsync(
        string to,
        string userName,
        string resetToken,
        CancellationToken cancellationToken = default);
}
