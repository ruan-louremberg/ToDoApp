namespace ToDoApp.Application.DTO.Auth;

/// <summary>Requests a password reset for the supplied account email.</summary>
public sealed record ForgotPasswordRequest
{
    public string Email { get; init; } = string.Empty;
}
