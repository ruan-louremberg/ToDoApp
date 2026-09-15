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

    private readonly DeleteCategoryUseCase _deleteCategoryUseCase;
    public CategoryController
    (
        CreateCategoryUseCase createCategoryUseCase,

        ListCategoriesUseCase listCategoriesUseCase,

        DeleteCategoryUseCase deleteCategoryUseCase
    )

    {
        _createCategoryUseCase = createCategoryUseCase;

        _listCategoriesUseCase = listCategoriesUseCase;
        
        _deleteCategoryUseCase = deleteCategoryUseCase;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]

    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var category = await _createCategoryUseCase.ExecuteAsync(request);

        if (category.IsFailed)
        {
            return Conflict(category.Errors[0].Message);
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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var result = await _deleteCategoryUseCase.ExecuteAsync(id);
        if (result.IsFailed)
        {
            return NotFound(result.Errors[0].Message);
        }
        return Ok();
    }

}