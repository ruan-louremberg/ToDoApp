namespace ToDoApp.Application.Interfaces;

public interface IPasswordService
{
    string Hash(string password);
}
