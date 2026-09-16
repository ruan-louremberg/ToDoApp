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
    private readonly GetTrashCategoryUseCase _getTrashCategoryUseCase;
    private readonly RestoreCategoryUseCase _restoreCategoryUseCase;
    public CategoryController
    (
        CreateCategoryUseCase createCategoryUseCase,

        ListCategoriesUseCase listCategoriesUseCase,

        DeleteCategoryUseCase deleteCategoryUseCase,

        GetTrashCategoryUseCase getTrashCategoryUseCase,

        RestoreCategoryUseCase restoreCategoryUseCase
    )

    {
        _createCategoryUseCase = createCategoryUseCase;

        _listCategoriesUseCase = listCategoriesUseCase;

        _deleteCategoryUseCase = deleteCategoryUseCase;

        _getTrashCategoryUseCase = getTrashCategoryUseCase;

        _restoreCategoryUseCase = restoreCategoryUseCase;
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
    
    [HttpGet("trash")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrash()
    {
        var categories = await _getTrashCategoryUseCase.ExecuteAsync();

        return Ok(categories);
    }


    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RestoreCategory(
        [FromRoute] Guid id)
    {
        var result = await _restoreCategoryUseCase.ExecuteAsync(id);

        if (result.IsFailed)
        {
            if (result.Errors[0].Message == "Categoria não encontrada na lixeira.")
            {
                return NotFound(result.Errors[0].Message);
            }
            return Conflict(result.Errors[0].Message);
        }

        return NoContent();
    }

}