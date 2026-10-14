using System.Text;
using System.Text.Encodings.Web;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class SendDueDateRemindersUseCase
{
    private readonly IToDoRepository _todoRepository;
    private readonly IEmailService _emailService;
    private readonly string _frontendBaseUrl;

    public SendDueDateRemindersUseCase(
        IToDoRepository todoRepository,
        IEmailService emailService,
        string frontendBaseUrl)
    {
        _todoRepository = todoRepository;
        _emailService = emailService;
        _frontendBaseUrl = frontendBaseUrl;
    }

    public async Task<int> ExecuteAsync(DateTime targetDate, CancellationToken cancellationToken = default)
    {
        var startOfDay = targetDate.Date;
        var startOfNextDay = startOfDay.AddDays(1);

        var tasks = await _todoRepository.GetPendingTasksDueBetweenAsync(
            startOfDay,
            startOfNextDay,
            cancellationToken);
        if (tasks.Count == 0)
        {
            return 0;
        }

        var tasksByUser = tasks.GroupBy(t => t.UserId);
        var emailsSent = 0;

        foreach (var userGroup in tasksByUser)
        {
            var userTasks = userGroup.ToList();
            var user = userTasks.First().User;
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                continue;
            }

            var subject = userTasks.Count == 1
                ? "Lembrete: você tem uma tarefa vencendo hoje"
                : $"Lembrete: você tem {userTasks.Count} tarefas vencendo hoje";

            var htmlBody = BuildEmailTemplate(user.Name, userTasks);

            await _emailService.SendEmailAsync(user.Email, subject, htmlBody, cancellationToken);

            foreach (var task in userTasks)
            {
                task.MarkReminderSent(DateTime.UtcNow);
                await _todoRepository.UpdateAsync(task, cancellationToken);
            }

            emailsSent++;
        }

        return emailsSent;
    }

    private string BuildEmailTemplate(string userName, List<ToDo> tasks)
    {
        var sb = new StringBuilder();
        var encodedUserName = HtmlEncoder.Default.Encode(userName);
        var tasksUrl = HtmlEncoder.Default.Encode($"{_frontendBaseUrl.TrimEnd('/')}/tasks");
        sb.Append($@"
        <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 24px; background-color: #f9fafb; border-radius: 8px;"">
            <div style=""text-align: center; margin-bottom: 24px;"">
                <h1 style=""color: #111827; margin: 0; font-size: 26px;"">todo<span style=""color: #3b82f6;"">.</span></h1>
                <p style=""color: #6b7280; font-size: 14px; margin-top: 4px;"">Lembrete de Tarefas Diárias</p>
            </div>
            
            <div style=""background: #ffffff; padding: 24px; border-radius: 8px; border: 1px solid #e5e7eb;"">
                <h2 style=""color: #1f2937; font-size: 18px; margin-top: 0;"">Olá, {encodedUserName}!</h2>
                <p style=""color: #4b5563; font-size: 14px; line-height: 1.5;"">
                    Este é um aviso automático para lembrá-lo que você tem <strong>{tasks.Count} tarefa{(tasks.Count > 1 ? "s" : "")}</strong> com prazo de entrega para <strong>hoje</strong>:
                </p>

                <div style=""margin: 20px 0;"">");

        foreach (var task in tasks)
        {
            var encodedTitle = HtmlEncoder.Default.Encode(task.Title);
            var priorityColor = task.Priority switch
            {
                Priority.High => "#ef4444",
                Priority.Medium => "#f59e0b",
                _ => "#10b981"
            };

            var priorityText = task.Priority switch
            {
                Priority.High => "Alta Prioridade",
                Priority.Medium => "Média Prioridade",
                _ => "Baixa Prioridade"
            };

            var categoryBadge = task.Category != null
                ? $@"<span style=""background: #e0f2fe; color: #0369a1; padding: 2px 8px; border-radius: 12px; font-size: 11px; margin-left: 8px;"">{HtmlEncoder.Default.Encode(task.Category.Name)}</span>"
                : "";

            sb.Append($@"
                <div style=""padding: 12px 16px; margin-bottom: 12px; background: #f3f4f6; border-left: 4px solid {priorityColor}; border-radius: 4px;"">
                    <div style=""font-weight: 600; font-size: 15px; color: #111827;"">
                        {encodedTitle}
                        {categoryBadge}
                    </div>");

            if (!string.IsNullOrWhiteSpace(task.Description))
            {
                sb.Append($@"<div style=""color: #6b7280; font-size: 13px; margin-top: 4px;"">{HtmlEncoder.Default.Encode(task.Description)}</div>");
            }

            sb.Append($@"
                    <div style=""color: {priorityColor}; font-size: 12px; margin-top: 6px; font-weight: 500;"">
                        • {priorityText}
                    </div>
                </div>");
        }

        sb.Append($@"
                </div>

                <div style=""text-align: center; margin-top: 24px;"">
                    <a href=""{tasksUrl}"" style=""display: inline-block; padding: 12px 24px; background-color: #2563eb; color: #ffffff; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px;"">
                        Acessar minhas tarefas
                    </a>
                </div>
            </div>

            <div style=""text-align: center; margin-top: 24px; color: #9ca3af; font-size: 12px;"">
                <p>Enviado automaticamente pelo seu <strong>todo.</strong> app às 08:00, horário de Brasília.</p>
            </div>
        </div>");

        return sb.ToString();
    }
}
