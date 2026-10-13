namespace ToDoApp.Application.DTO.Auth;

public record UserResponse(
    Guid Id,
    string Name,
    string Email);
