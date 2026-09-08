using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.UseCases;
using ToDoApp.Application.DTO;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/task")]

public class TaskController(CreateTaskUseCase createTaskUseCase, ListTasksUseCase listTasksUseCase) : ControllerBase
{
    private readonly CreateTaskUseCase _createTaskUseCase = createTaskUseCase;
    private readonly ListTasksUseCase _listTasksUseCase = listTasksUseCase;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var task = await _createTaskUseCase.ExecuteAsync(request);

        if (task.IsFailed)
        {
            return BadRequest(task.Errors[0].Message);
        }
        return Ok(task);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] ListTasksRequest request)
    {
        var tasks = await _listTasksUseCase.ExecuteAsync(request);
        return Ok(tasks);
    }
}