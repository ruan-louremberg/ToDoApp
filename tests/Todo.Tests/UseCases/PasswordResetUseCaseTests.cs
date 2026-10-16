using Moq;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.UseCases;
using ToDoApp.Application.Validators;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Tests.UseCases;

public class PasswordResetUseCaseTests
{
    [Fact]
    public async Task GivenUnknownEmail_WhenRequestingReset_ThenReturnsSameSuccessfulResultWithoutSendingEmail()
    {
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.GetByEmailAsync("unknown@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        var emailService = new Mock<IEmailService>();

        var result = await new ForgotPasswordUseCase(
                repository.Object,
                emailService.Object,
                new ForgotPasswordRequestValidator())
            .ExecuteAsync(new ForgotPasswordRequest { Email = "unknown@example.com" });

        Assert.True(result.IsSuccess);
        emailService.Verify(
            item => item.SendPasswordResetEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GivenExistingUser_WhenRequestingReset_ThenStoresOnlyTokenHashAndSendsRawToken()
    {
        var user = new User("Maria", "maria@example.com", "old-password-hash");
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.GetByEmailAsync("maria@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        repository
            .Setup(item => item.TrySchedulePasswordResetAsync(
                user.Email,
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var emailService = new Mock<IEmailService>();
        string? emailedToken = null;
        emailService
            .Setup(item => item.SendPasswordResetEmailAsync(
                user.Email,
                user.Name,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, string, string, CancellationToken>((_, _, token, _) => emailedToken = token)
            .Returns(Task.CompletedTask);

        var result = await new ForgotPasswordUseCase(
                repository.Object,
                emailService.Object,
                new ForgotPasswordRequestValidator())
            .ExecuteAsync(new ForgotPasswordRequest { Email = user.Email });

        Assert.True(result.IsSuccess);
        Assert.NotNull(emailedToken);
        Assert.Equal(64, emailedToken.Length);
        Assert.NotEqual(emailedToken, user.PasswordResetTokenHash);
        Assert.NotNull(user.PasswordResetTokenExpiresAt);
        Assert.InRange(
            user.PasswordResetTokenExpiresAt.Value,
            DateTime.UtcNow.AddMinutes(14),
            DateTime.UtcNow.AddMinutes(16));
        repository.Verify(
            item => item.TrySchedulePasswordResetAsync(
                user.Email,
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        emailService.Verify(
            item => item.SendPasswordResetEmailAsync(
                user.Email,
                user.Name,
                emailedToken,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenRecentResetRequest_WhenRequestingAgain_ThenDoesNotSendAnotherEmail()
    {
        var user = new User("Maria", "maria@example.com", "old-password-hash");
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        repository
            .Setup(item => item.TrySchedulePasswordResetAsync(
                user.Email,
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var emailService = new Mock<IEmailService>();

        var result = await new ForgotPasswordUseCase(
                repository.Object,
                emailService.Object,
                new ForgotPasswordRequestValidator())
            .ExecuteAsync(new ForgotPasswordRequest { Email = user.Email });

        Assert.True(result.IsSuccess);
        emailService.Verify(
            item => item.SendPasswordResetEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GivenValidToken_WhenResettingPassword_ThenConsumesTokenAndStoresPasswordHash()
    {
        var token = new string('A', 64);
        const string passwordHash = "new-password-hash";
        const string currentPasswordHash = "old-password-hash";
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.GetPasswordHashForValidPasswordResetTokenAsync(
                It.Is<string>(hash => hash.Length == 64),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentPasswordHash);
        repository
            .Setup(item => item.ResetPasswordAsync(
                It.Is<string>(hash => hash.Length == 64),
                passwordHash,
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var passwordService = new Mock<IPasswordService>();
        passwordService
            .Setup(item => item.Verify("new-password", currentPasswordHash))
            .Returns(false);
        passwordService.Setup(item => item.Hash("new-password")).Returns(passwordHash);

        var result = await new ResetPasswordUseCase(
                repository.Object,
                passwordService.Object,
                new ResetPasswordRequestValidator())
            .ExecuteAsync(new ResetPasswordRequest
            {
                Token = token,
                Password = "new-password"
            });

        Assert.True(result.IsSuccess);
        passwordService.Verify(
            item => item.Verify("new-password", currentPasswordHash),
            Times.Once);
        repository.Verify(
            item => item.ResetPasswordAsync(
                It.IsAny<string>(),
                passwordHash,
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GivenExpiredToken_WhenResettingPassword_ThenRejectsItWithoutHashingPassword()
    {
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.HasValidPasswordResetTokenAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var passwordService = new Mock<IPasswordService>();

        var result = await new ResetPasswordUseCase(
                repository.Object,
                passwordService.Object,
                new ResetPasswordRequestValidator())
            .ExecuteAsync(new ResetPasswordRequest
            {
                Token = new string('A', 64),
                Password = "new-password"
            });

        Assert.True(result.IsFailed);
        Assert.Equal(400, result.Errors[0].Metadata["statusCode"]);
        passwordService.Verify(item => item.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        passwordService.Verify(item => item.Hash(It.IsAny<string>()), Times.Never);
        repository.Verify(
            item => item.ResetPasswordAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GivenSamePassword_WhenResettingPassword_ThenRejectsItWithShortMessage()
    {
        const string currentPasswordHash = "old-password-hash";
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(item => item.GetPasswordHashForValidPasswordResetTokenAsync(
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentPasswordHash);
        var passwordService = new Mock<IPasswordService>();
        passwordService
            .Setup(item => item.Verify("same-password", currentPasswordHash))
            .Returns(true);

        var result = await new ResetPasswordUseCase(
                repository.Object,
                passwordService.Object,
                new ResetPasswordRequestValidator())
            .ExecuteAsync(new ResetPasswordRequest
            {
                Token = new string('A', 64),
                Password = "same-password"
            });

        Assert.True(result.IsFailed);
        Assert.Equal("Escolha uma senha diferente da atual.", result.Errors[0].Message);
        Assert.Equal(400, result.Errors[0].Metadata["statusCode"]);
        passwordService.Verify(item => item.Hash(It.IsAny<string>()), Times.Never);
        repository.Verify(
            item => item.ResetPasswordAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
