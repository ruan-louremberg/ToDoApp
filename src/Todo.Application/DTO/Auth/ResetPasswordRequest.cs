namespace ToDoApp.Application.DTO.Auth;

/// <summary>Submits a one-time password reset token and the new password.</summary>
public sealed record ResetPasswordRequest
{
    public string Token { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
