namespace ToDoApp.Application.DTO.Auth;

public record RegisterRequest
{
    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
