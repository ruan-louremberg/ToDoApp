using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class LoginUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IValidator<LoginRequest> _validator;

    public LoginUserUseCase(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IValidator<LoginRequest> validator)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _validator = validator;
    }

    public async Task<Result<UserResponse>> ExecuteAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordService.Verify(request.Password, user.PasswordHash))
        {
            return Result.Fail(new Error("E-mail ou senha inválidos.")
                .WithMetadata("statusCode", 401));
        }

        return Result.Ok(new UserResponse(user.Id, user.Name, user.Email));
    }
}
