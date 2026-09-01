using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ToDoApp.Api.DTO;
using ToDoApp.Application.UseCases;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/createtask")]

public class TaskController(CreateTaskUseCase createTaskUseCase) : ControllerBase
{
    private readonly CreateTaskUseCase _createTaskUseCase = createTaskUseCase;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var task = await _createTaskUseCase.ExecuteAsync(request.Title, request.Description, request.Priority, request.DueDate);
        return Ok(task);
    }
}