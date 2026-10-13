using FluentResults;
using FluentValidation;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IValidator<RegisterRequest> _validator;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IValidator<RegisterRequest> validator)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _validator = validator;
    }

    public async Task<Result<UserResponse>> ExecuteAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.Select(error =>
                new Error(error.ErrorMessage).WithMetadata("statusCode", 400)));
        }

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            return Result.Fail(new Error("Este e-mail já está cadastrado.")
                .WithMetadata("statusCode", 409));
        }

        var user = new User(
            request.Name.Trim(),
            email,
            _passwordService.Hash(request.Password));

        await _userRepository.AddAsync(user, cancellationToken);

        return Result.Ok(new UserResponse(user.Id, user.Name, user.Email));
    }
}
