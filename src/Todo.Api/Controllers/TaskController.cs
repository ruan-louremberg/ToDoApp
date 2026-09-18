using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using ToDoApp.Application.UseCases;
using ToDoApp.Application.DTO;
namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/task")]

public class TaskController(CreateTaskUseCase createTaskUseCase, UpdateTaskUseCase updateTaskUseCase, ListTasksUseCase listTasksUseCase,
 CompleteTaskUseCase completeTaskUseCase, DeleteTaskUseCase deleteTaskUseCase, GetTrashUseCase getTrashUseCase,
    RestoreTaskUseCase restoreTaskUseCase, GetSummaryUseCase getSummaryUseCase) : ApiControllerBase
{

    private readonly CreateTaskUseCase _createTaskUseCase = createTaskUseCase;

    private readonly ListTasksUseCase _listTasksUseCase = listTasksUseCase;

    private readonly UpdateTaskUseCase _updateTaskUseCase = updateTaskUseCase;

    private readonly CompleteTaskUseCase _completeTaskUseCase = completeTaskUseCase;

    private readonly DeleteTaskUseCase _deleteTaskUseCase = deleteTaskUseCase;

    private readonly GetTrashUseCase _getTrashUseCase = getTrashUseCase;

    private readonly RestoreTaskUseCase _restoreTaskUseCase = restoreTaskUseCase;
    private readonly IValidator<ListTasksRequest> _listTasksValidator = listTasksValidator;

    private readonly GetSummaryUseCase _getSummaryUseCase = getSummaryUseCase;

    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var task = await _createTaskUseCase.ExecuteAsync(request);

        if (task.IsFailed)
        {
            return FromErrors(task.Errors);
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
            return FromErrors(result.Errors);
        }
        return Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] ListTasksRequest request)
    {
        var validationResult = await _listTasksValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FromValidationErrors(validationResult.Errors);
        }

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
            return FromErrors(result.Errors);
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
            return FromErrors(result.Errors);
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
            return FromErrors(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await _getSummaryUseCase.ExecuteAsync(cancellationToken);
        return Ok(summary);
    }
}