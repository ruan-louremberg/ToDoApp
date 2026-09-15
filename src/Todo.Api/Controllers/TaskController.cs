using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.UseCases;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Exceptions;
namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/task")]

public class TaskController(CreateTaskUseCase createTaskUseCase, UpdateTaskUseCase updateTaskUseCase, ListTasksUseCase listTasksUseCase,
 CompleteTaskUseCase completeTaskUseCase, DeleteTaskUseCase deleteTaskUseCase, GetTrashUseCase getTrashUseCase,
  RestoreTaskUseCase restoreTaskUseCase) : ControllerBase
{

    private readonly CreateTaskUseCase _createTaskUseCase = createTaskUseCase;
    private readonly ListTasksUseCase _listTasksUseCase = listTasksUseCase;
    private readonly UpdateTaskUseCase _updateTaskUseCase = updateTaskUseCase;
    private readonly CompleteTaskUseCase _completeTaskUseCase = completeTaskUseCase;

    private readonly DeleteTaskUseCase _deleteTaskUseCase = deleteTaskUseCase;

    private readonly GetTrashUseCase _getTrashUseCase = getTrashUseCase;

    private readonly RestoreTaskUseCase _restoreTaskUseCase = restoreTaskUseCase;

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var task = await _createTaskUseCase.ExecuteAsync(request);

        if (task.IsFailed)
        {
            return BadRequest(task.Errors[0].Message);
        }
        return CreatedAtAction(nameof(CreateTask), new { id = task.Value.Id }, task.Value);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]


    public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] UpdateTaskRequest request)

    {
        var result = await _updateTaskUseCase.ExecuteAsync(id, request);

        if (result.IsFailed)
        {
            // Retorna 400 Bad Request com as mensagens do FluentResults
            return BadRequest(result.Errors.Select(e => e.Message));
        }
        return Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] ListTasksRequest request)
    {
        var tasks = await _listTasksUseCase.ExecuteAsync(request);
        return Ok(tasks);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTask([FromRoute] Guid id, [FromBody] CompleteTaskRequest request)
    {
        var result = await _completeTaskUseCase.ExecuteAsync(id, request);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(
        [FromRoute] Guid id)
    {
        var result = await _deleteTaskUseCase.ExecuteAsync(id);

        if (result.IsFailed)
        {
            return NotFound(result.Errors[0].Message);
        }

        return NoContent();
    }


    [HttpGet("trash")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrash()
    {
        var tasks = await _getTrashUseCase.ExecuteAsync();

        return Ok(tasks);
    }


    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreTask(
        [FromRoute] Guid id)
    {
        var result = await _restoreTaskUseCase.ExecuteAsync(id);

        if (result.IsFailed)
        {
            return NotFound(result.Errors[0].Message);
        }

        return NoContent();
    }
}