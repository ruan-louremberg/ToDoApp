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
        user.SetPasswordResetToken(
            PasswordResetTokenHasher.Hash(token),
            DateTime.UtcNow.Add(TokenLifetime));
        await userRepository.UpdateAsync(user, cancellationToken);
        await emailService.SendPasswordResetEmailAsync(
            user.Email,
            user.Name,
            token,
            cancellationToken);

        return Result.Ok();
    }
}
