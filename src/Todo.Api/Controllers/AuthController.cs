using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.Interfaces;
using ToDoApp.Application.UseCases;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly LoginUserUseCase _loginUserUseCase;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    private readonly ForgotPasswordUseCase _forgotPasswordUseCase;
    private readonly ResetPasswordUseCase _resetPasswordUseCase;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        RegisterUserUseCase registerUserUseCase,
        LoginUserUseCase loginUserUseCase,
        IJwtTokenService jwtTokenService,
        IUserRepository userRepository,
        ForgotPasswordUseCase forgotPasswordUseCase,
        ResetPasswordUseCase resetPasswordUseCase,
        ILogger<AuthController> logger)
    {
        _registerUserUseCase = registerUserUseCase;
        _loginUserUseCase = loginUserUseCase;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _forgotPasswordUseCase = forgotPasswordUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(PasswordResetRequestedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _forgotPasswordUseCase.ExecuteAsync(request, cancellationToken);
            if (result.IsFailed)
            {
                return FromErrors(result.Errors);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (EmailDeliveryException exception)
        {
            _logger.LogError(exception, "Password reset email could not be sent.");
        }

        return Ok(new PasswordResetRequestedResponse(
            "Se o e-mail estiver cadastrado, você receberá instruções para redefinir a senha."));
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _resetPasswordUseCase.ExecuteAsync(request, cancellationToken);
        return result.IsFailed
            ? FromErrors(result.Errors)
            : NoContent();
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _registerUserUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsFailed)
        {
            return FromErrors(result.Errors);
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.Value);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _loginUserUseCase.ExecuteAsync(request, cancellationToken);
        if (result.IsFailed)
        {
            return FromErrors(result.Errors);
        }

        var user = result.Value;
        var token = _jwtTokenService.CreateToken(
            user.Id,
            user.Name,
            user.Email);

        Response.Cookies.Append("todo_auth", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(2),
            IsEssential = true
        });

        return Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(CurrentUserId, cancellationToken);
        if (user is null)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Sessão inválida",
                Detail = "Faça login novamente para continuar.",
                Instance = HttpContext.Request.Path
            };
            problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;
            return Unauthorized(problemDetails);
        }

        return Ok(new UserResponse(user.Id, user.Name, user.Email));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("todo_auth", new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            IsEssential = true
        });

        return NoContent();
    }
}
