namespace ToDoApp.Application.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(Guid userId, string name, string email);
}
