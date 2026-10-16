using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resend;
using ToDoApp.Application.Interfaces;
using System.Net;

namespace ToDoApp.Infrastructure.Services;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly string? _apiKey;
    private readonly string _fromEmail;
    private readonly string? _devRecipientEmail;
    private readonly string _frontendBaseUrl;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(
        IResend resend,
        IConfiguration configuration,
        ILogger<ResendEmailService> logger)
    {
        _resend = resend;
        _logger = logger;
        _apiKey = configuration["Resend:ApiKey"]
            ?? Environment.GetEnvironmentVariable("RESEND_APIKEY")
            ?? Environment.GetEnvironmentVariable("RESEND_API_KEY");
        _fromEmail = configuration["Resend:FromEmail"] ?? "onboarding@resend.dev";
        _devRecipientEmail = configuration["Resend:DevRecipientEmail"];
        _frontendBaseUrl = configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new EmailDeliveryException(
                "Configure Resend:ApiKey ou a variável de ambiente RESEND_APIKEY antes de enviar e-mails.");
        }

        var targetRecipient = string.IsNullOrWhiteSpace(_devRecipientEmail)
            ? to
            : _devRecipientEmail;

        try
        {
            var message = new EmailMessage
            {
                From = _fromEmail,
                To = targetRecipient,
                Subject = subject,
                HtmlBody = htmlBody
            };

            await _resend.EmailSendAsync(message, cancellationToken);
            _logger.LogInformation("E-mail enviado com sucesso via Resend para {Recipient} (destinatário original: {Original})", targetRecipient, to);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail via Resend para {Recipient}", targetRecipient);
            throw new EmailDeliveryException("O serviço de e-mail não conseguiu enviar a mensagem.", ex);
        }
    }

    public Task SendPasswordResetEmailAsync(
        string to,
        string userName,
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        var resetUrl = $"{_frontendBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(resetToken)}";
        var encodedName = WebUtility.HtmlEncode(userName);
        var encodedUrl = WebUtility.HtmlEncode(resetUrl);
        var htmlBody = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 24px;">
                <h1>Redefinição de senha</h1>
                <p>Olá, {encodedName}. Recebemos uma solicitação para redefinir a senha da sua conta.</p>
                <p><a href="{encodedUrl}">Criar uma nova senha</a></p>
                <p>Este link expira em 15 minutos e só pode ser usado uma vez. Se você não solicitou a redefinição, ignore esta mensagem.</p>
            </div>
            """;

        return SendEmailAsync(
            to,
            "Redefinição de senha do todo.",
            htmlBody,
            cancellationToken);
    }
}
