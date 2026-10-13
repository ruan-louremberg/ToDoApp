using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskId:guid}/subtasks")]
[Authorize]
public sealed class SubTasksController(
    ListSubTasksUseCase listSubTasksUseCase,
    GetSubTaskUseCase getSubTaskUseCase,
    CreateSubTaskUseCase createSubTaskUseCase,
    UpdateSubTaskUseCase updateSubTaskUseCase,
    DeleteSubTaskUseCase deleteSubTaskUseCase,
    ReorderSubTasksUseCase reorderSubTasksUseCase) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SubTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListSubTasks(
        [FromRoute] Guid taskId,
        CancellationToken cancellationToken)
    {
        var result = await listSubTasksUseCase.ExecuteAsync(CurrentUserId, taskId, cancellationToken);
        return result.IsFailed ? FromErrors(result.Errors) : Ok(result.Value);
    }

    [HttpGet("{subtaskId:guid}")]
    [ProducesResponseType(typeof(SubTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubTask(
        [FromRoute] Guid taskId,
        [FromRoute] Guid subtaskId,
        CancellationToken cancellationToken)
    {
        var result = await getSubTaskUseCase.ExecuteAsync(
            CurrentUserId, taskId, subtaskId, cancellationToken);
        return result.IsFailed ? FromErrors(result.Errors) : Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSubTask(
        [FromRoute] Guid taskId,
        [FromBody] CreateSubTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createSubTaskUseCase.ExecuteAsync(
            CurrentUserId, taskId, request, cancellationToken);
        if (result.IsFailed)
        {
            return FromErrors(result.Errors);
        }

        return CreatedAtAction(
            nameof(GetSubTask),
            new { taskId, subtaskId = result.Value.Id },
            result.Value);
    }

    [HttpPatch("{subtaskId:guid}")]
    [ProducesResponseType(typeof(SubTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubTask(
        [FromRoute] Guid taskId,
        [FromRoute] Guid subtaskId,
        [FromBody] UpdateSubTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateSubTaskUseCase.ExecuteAsync(
            CurrentUserId, taskId, subtaskId, request, cancellationToken);
        return result.IsFailed ? FromErrors(result.Errors) : Ok(result.Value);
    }

    [HttpDelete("{subtaskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubTask(
        [FromRoute] Guid taskId,
        [FromRoute] Guid subtaskId,
        CancellationToken cancellationToken)
    {
        var result = await deleteSubTaskUseCase.ExecuteAsync(
            CurrentUserId, taskId, subtaskId, cancellationToken);
        return result.IsFailed ? FromErrors(result.Errors) : NoContent();
    }

    [HttpPatch("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderSubTasks(
        [FromRoute] Guid taskId,
        [FromBody] ReorderSubTasksRequest request,
        CancellationToken cancellationToken)
    {
        var result = await reorderSubTasksUseCase.ExecuteAsync(
            CurrentUserId, taskId, request, cancellationToken);
        return result.IsFailed ? FromErrors(result.Errors) : NoContent();
    }
}
