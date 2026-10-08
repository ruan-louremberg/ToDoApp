using ToDoApp.Domain.Entities;

namespace ToDoApp.Tests.Entities;

public class UserTests
{
    [Fact(DisplayName = "Inicializa usuário com nome, e-mail e hash da senha")]
    public void GivenUserData_WhenConstructed_ThenStoresIdentityData()
    {
        var user = new User("Maria", "user@example.com", "password-hash");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Maria", user.Name);
        Assert.Equal("user@example.com", user.Email);
        Assert.Equal("password-hash", user.PasswordHash);
        Assert.Null(user.UpdatedAt);
    }

    [Fact(DisplayName = "Atualiza o hash da senha e a data de alteração")]
    public void GivenNewPassword_WhenUpdatingPassword_ThenStoresHashAndUpdateDate()
    {
        var user = new User("Maria", "user@example.com", "old-hash");

        user.UpdatePassword("new-hash");

        Assert.Equal("new-hash", user.PasswordHash);
        Assert.NotNull(user.UpdatedAt);
    }
}
