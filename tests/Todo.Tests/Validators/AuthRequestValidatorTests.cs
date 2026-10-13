using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Validators;

namespace ToDoApp.Tests.Validators;

public class AuthRequestValidatorTests
{
    [Fact(DisplayName = "Cadastro inválido retorna mensagens de validação mesmo com campos nulos")]
    public async Task GivenNullOrEmptyFields_WhenValidatingRegistration_ThenReturnsFriendlyMessages()
    {
        var validator = new RegisterRequestValidator();

        var result = await validator.ValidateAsync(new RegisterRequest
        {
            Name = null!,
            Email = string.Empty,
            Password = string.Empty
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(RegisterRequest.Name) &&
            error.ErrorMessage == "O nome é obrigatório.");
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(RegisterRequest.Email) &&
            error.ErrorMessage == "O e-mail é obrigatório.");
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(RegisterRequest.Password) &&
            error.ErrorMessage == "A senha é obrigatória.");
    }

    [Fact(DisplayName = "Login inválido retorna mensagens de validação")]
    public async Task GivenEmptyFields_WhenValidatingLogin_ThenReturnsFriendlyMessages()
    {
        var validator = new LoginRequestValidator();

        var result = await validator.ValidateAsync(new LoginRequest());

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(LoginRequest.Email) &&
            error.ErrorMessage == "O e-mail é obrigatório.");
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(LoginRequest.Password) &&
            error.ErrorMessage == "A senha é obrigatória.");
    }

    [Theory(DisplayName = "Rejeita e-mails sem domínio válido no cadastro")]
    [InlineData("teodosio@g")]
    [InlineData("user@domain.")]
    [InlineData("user@domain.c")]
    public async Task GivenEmailWithoutValidDomain_WhenValidatingRegistration_ThenReturnsEmailError(
        string email)
    {
        var result = await new RegisterRequestValidator().ValidateAsync(new RegisterRequest
        {
            Name = "Teodosio",
            Email = email,
            Password = "valid-password"
        });

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(RegisterRequest.Email) &&
            error.ErrorMessage == "O e-mail informado é inválido.");
    }

    [Theory(DisplayName = "Rejeita e-mails sem domínio válido no login")]
    [InlineData("teodosio@g")]
    [InlineData("user@domain.")]
    [InlineData("user@domain.c")]
    public async Task GivenEmailWithoutValidDomain_WhenValidatingLogin_ThenReturnsEmailError(
        string email)
    {
        var result = await new LoginRequestValidator().ValidateAsync(new LoginRequest
        {
            Email = email,
            Password = "valid-password"
        });

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(LoginRequest.Email) &&
            error.ErrorMessage == "O e-mail informado é inválido.");
    }

    [Fact(DisplayName = "Aceita e-mail com domínio e sufixo válidos")]
    public async Task GivenValidEmail_WhenValidatingAuthRequests_ThenReturnsSuccess()
    {
        var registrationResult = await new RegisterRequestValidator().ValidateAsync(new RegisterRequest
        {
            Name = "Teodosio",
            Email = "teodosio@example.com",
            Password = "valid-password"
        });
        var loginResult = await new LoginRequestValidator().ValidateAsync(new LoginRequest
        {
            Email = "teodosio@example.com",
            Password = "valid-password"
        });

        Assert.True(registrationResult.IsValid);
        Assert.True(loginResult.IsValid);
    }
}
