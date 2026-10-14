namespace ToDoApp.Application.DTO.Auth;

/// <summary>Generic response that does not reveal whether an email is registered.</summary>
public sealed record PasswordResetRequestedResponse(string Message);
