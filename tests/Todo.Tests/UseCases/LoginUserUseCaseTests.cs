using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class LoginUserUseCaseTests
{
    [Fact(DisplayName = "Retorna erro quando os dados do login são inválidos")]
    public async Task GivenInvalidRequest_WhenLoggingIn_ThenReturnsBadRequest()
    {
        var repository = new Mock<IUserRepository>();
        var passwordService = new Mock<IPasswordService>();

        var result = await CreateSut(repository, passwordService, CreateValidator(false))
            .ExecuteAsync(new LoginRequest());

        Assert.True(result.IsFailed);
        Assert.Equal(400, result.Errors[0].Metadata["statusCode"]);
        repository.Verify(
            item => item.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        passwordService.Verify(
            item => item.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact(DisplayName = "Retorna erro genérico quando as credenciais são inválidas")]
    public async Task GivenInvalidCredentials_WhenLoggingIn_ThenReturnsUnauthorized()
    {
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.GetByEmailAsync(
                "maria@example.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await CreateSut(repository)
            .ExecuteAsync(new LoginRequest
            {
                Email = " Maria@EXAMPLE.com ",
                Password = "wrong-password"
            });

        Assert.True(result.IsFailed);
        Assert.Equal(401, result.Errors[0].Metadata["statusCode"]);
        Assert.Equal("E-mail ou senha inválidos.", result.Errors[0].Message);
    }

    [Fact(DisplayName = "Retorna o usuário quando as credenciais são válidas")]
    public async Task GivenValidCredentials_WhenLoggingIn_ThenReturnsUser()
    {
        var user = new User("Maria", "maria@example.com", "hashed-password");
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.GetByEmailAsync(
                "maria@example.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var passwordService = new Mock<IPasswordService>();
        passwordService
            .Setup(item => item.Verify("password123", "hashed-password"))
            .Returns(true);

        var result = await CreateSut(repository, passwordService)
            .ExecuteAsync(new LoginRequest
            {
                Email = " Maria@EXAMPLE.com ",
                Password = "password123"
            });

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal("Maria", result.Value.Name);
        Assert.Equal("maria@example.com", result.Value.Email);
    }

    private static LoginUserUseCase CreateSut(
        Mock<IUserRepository> repository,
        Mock<IPasswordService>? passwordService = null,
        Mock<IValidator<LoginRequest>>? validator = null)
    {
        passwordService ??= new Mock<IPasswordService>();
        validator ??= CreateValidator(true);
        return new LoginUserUseCase(
            repository.Object,
            passwordService.Object,
            validator.Object);
    }

    private static Mock<IValidator<LoginRequest>> CreateValidator(bool isValid)
    {
        var validator = new Mock<IValidator<LoginRequest>>();
        validator
            .Setup(item => item.ValidateAsync(
                It.IsAny<LoginRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(isValid
                ? new ValidationResult()
                : new ValidationResult(new[]
                {
                    new ValidationFailure("Request", "Invalid")
                }));
        return validator;
    }
}
