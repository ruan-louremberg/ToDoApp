using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTO.Auth;
using ToDoApp.Application.UseCases;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;

    public AuthController(RegisterUserUseCase registerUserUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
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
}
