using FluentValidation;
using FluentValidation.Results;
using Moq;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class RegisterUserUseCaseTests
{
    [Fact(DisplayName = "Retorna erro quando os dados do cadastro são inválidos")]
    public async Task GivenInvalidRequest_WhenRegisteringUser_ThenReturnsBadRequest()
    {
        var repository = new Mock<IUserRepository>();
        var validator = CreateValidator(false);
        var passwordService = new Mock<IPasswordService>();

        var result = await CreateSut(repository, validator, passwordService)
            .ExecuteAsync(new RegisterRequest());

        Assert.True(result.IsFailed);
        Assert.Equal(400, result.Errors[0].Metadata["statusCode"]);
        repository.Verify(
            item => item.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
        passwordService.Verify(item => item.Hash(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "Retorna conflito quando o e-mail já existe")]
    public async Task GivenExistingEmail_WhenRegisteringUser_ThenReturnsConflict()
    {
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.ExistsByEmailAsync(
                "maria@example.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await CreateSut(repository).ExecuteAsync(new RegisterRequest
        {
            Name = "Maria",
            Email = " Maria@EXAMPLE.com ",
            Password = "password123"
        });

        Assert.True(result.IsFailed);
        Assert.Equal(409, result.Errors[0].Metadata["statusCode"]);
    }

    [Fact(DisplayName = "Cria usuário com dados normalizados e senha em hash")]
    public async Task GivenValidRequest_WhenRegisteringUser_ThenPersistsHashedUser()
    {
        var repository = new Mock<IUserRepository>();
        var passwordService = new Mock<IPasswordService>();
        passwordService
            .Setup(item => item.Hash("password123"))
            .Returns("hashed-password");

        var result = await CreateSut(repository, passwordService: passwordService)
            .ExecuteAsync(new RegisterRequest
            {
                Name = " Maria ",
                Email = " Maria@EXAMPLE.com ",
                Password = "password123"
            });

        Assert.True(result.IsSuccess);
        Assert.Equal("Maria", result.Value.Name);
        Assert.Equal("maria@example.com", result.Value.Email);
        repository.Verify(
            item => item.AddAsync(
                It.Is<User>(user =>
                    user.Name == "Maria" &&
                    user.Email == "maria@example.com" &&
                    user.PasswordHash == "hashed-password"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static RegisterUserUseCase CreateSut(
        Mock<IUserRepository> repository,
        Mock<IValidator<RegisterRequest>>? validator = null,
        Mock<IPasswordService>? passwordService = null)
    {
        validator ??= CreateValidator(true);
        passwordService ??= new Mock<IPasswordService>();
        return new RegisterUserUseCase(
            repository.Object,
            passwordService.Object,
            validator.Object);
    }

    private static Mock<IValidator<RegisterRequest>> CreateValidator(bool isValid)
    {
        var validator = new Mock<IValidator<RegisterRequest>>();
        validator
            .Setup(item => item.ValidateAsync(
                It.IsAny<RegisterRequest>(),
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
