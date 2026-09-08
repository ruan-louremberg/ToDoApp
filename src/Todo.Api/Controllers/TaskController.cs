using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.UseCases;
using ToDoApp.Application.DTO;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/task")]

public class TaskController(CreateTaskUseCase createTaskUseCase, UpdateTaskUseCase updateTaskUseCase) : ControllerBase
{
    private readonly CreateTaskUseCase _createTaskUseCase = createTaskUseCase;

    private readonly UpdateTaskUseCase _updateTaskUseCase = updateTaskUseCase;

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var task = await _createTaskUseCase.ExecuteAsync(request);

        if (task.IsFailed)
        {
            return BadRequest(task.Errors[0].Message);
        }
        return Ok(task.Value);
    }
    
    [HttpPatch()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    

    public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskRequest request)
    {
        var result = await _updateTaskUseCase.ExecuteAsync(request);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors[0].Message;

            if (errorMessage == "Tarefa não encontrada.")
            {
                return NotFound(errorMessage);
            }

            return BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }
}