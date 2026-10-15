using System.Security.Cryptography;
using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.Security;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public sealed class ForgotPasswordUseCase(
    IUserRepository userRepository,
    IEmailService emailService,
    IValidator<ForgotPasswordRequest> validator)
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan RequestCooldown = TimeSpan.FromMinutes(1);

    public async Task<Result> ExecuteAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return Result.Ok();
        }

        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var requestedAt = DateTime.UtcNow;
        var tokenHash = PasswordResetTokenHasher.Hash(token);
        var scheduled = await userRepository.TrySchedulePasswordResetAsync(
            user.Email,
            tokenHash,
            requestedAt.Add(TokenLifetime),
            requestedAt,
            requestedAt.Subtract(RequestCooldown),
            cancellationToken);

        if (!scheduled)
        {
            return Result.Ok();
        }

        user.SetPasswordResetToken(
            tokenHash,
            requestedAt.Add(TokenLifetime),
            requestedAt);
        await emailService.SendPasswordResetEmailAsync(
            user.Email,
            user.Name,
            token,
            cancellationToken);

        return Result.Ok();
    }
}
