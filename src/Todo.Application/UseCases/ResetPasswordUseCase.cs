using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Security;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public sealed class ResetPasswordUseCase(
    IUserRepository userRepository,
    IPasswordService passwordService,
    IValidator<ResetPasswordRequest> validator)
{
    public async Task<Result> ExecuteAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        var tokenHash = PasswordResetTokenHasher.Hash(request.Token);
        var now = DateTime.UtcNow;
        if (!await userRepository.HasValidPasswordResetTokenAsync(
                tokenHash,
                now,
                cancellationToken))
        {
            return InvalidOrExpiredToken();
        }

        var passwordHash = passwordService.Hash(request.Password);
        var passwordWasReset = await userRepository.ResetPasswordAsync(
            tokenHash,
            passwordHash,
            now,
            cancellationToken);

        return passwordWasReset
            ? Result.Ok()
            : InvalidOrExpiredToken();
    }

    private static Result InvalidOrExpiredToken()
    {
        return Result.Fail(new Error("O link de redefinição é inválido ou expirou.")
            .WithMetadata("statusCode", 400));
    }
}
