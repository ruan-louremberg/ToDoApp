using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;

namespace ToDo.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly CreateCategoryUseCase _createCategoryUseCase;
    private readonly ListCategoriesUseCase _listCategoriesUseCase;

    public CategoryController(CreateCategoryUseCase createCategoryUseCase, ListCategoriesUseCase listCategoriesUseCase)
    {
        _createCategoryUseCase = createCategoryUseCase;
        _listCategoriesUseCase = listCategoriesUseCase;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var category = await _createCategoryUseCase.ExecuteAsync(request);

        if (category.IsFailed)
        {
            return BadRequest(category.Errors[0].Message);
        }
        return Ok(category.Value);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] ListCategoriesRequest request)
    {
        var categories = await _listCategoriesUseCase.ExecuteAsync(request);
        return Ok(categories);
    }

}