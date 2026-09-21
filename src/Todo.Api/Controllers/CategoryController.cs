using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using ToDoApp.Api.Controllers;
using ToDoApp.Application.DTO;
using ToDoApp.Application.UseCases;

namespace ToDo.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ApiControllerBase
{
    private readonly CreateCategoryUseCase _createCategoryUseCase;
    private readonly ListCategoriesUseCase _listCategoriesUseCase;
    private readonly UpdateCategoryUseCase _updateCategoryUseCase;
    private readonly DeleteCategoryUseCase _deleteCategoryUseCase;
    private readonly GetTrashCategoryUseCase _getTrashCategoryUseCase;
    private readonly RestoreCategoryUseCase _restoreCategoryUseCase;
    public CategoryController
    (
        CreateCategoryUseCase createCategoryUseCase,

        ListCategoriesUseCase listCategoriesUseCase,

        UpdateCategoryUseCase updateCategoryUseCase,

        DeleteCategoryUseCase deleteCategoryUseCase,

        GetTrashCategoryUseCase getTrashCategoryUseCase,

        RestoreCategoryUseCase restoreCategoryUseCase,
            IValidator<ListCategoriesRequest> listCategoriesValidator
    )

    {
        _createCategoryUseCase = createCategoryUseCase;

        _listCategoriesUseCase = listCategoriesUseCase;

        _updateCategoryUseCase = updateCategoryUseCase;

        _deleteCategoryUseCase = deleteCategoryUseCase;

        _getTrashCategoryUseCase = getTrashCategoryUseCase;

        _restoreCategoryUseCase = restoreCategoryUseCase;

        _listCategoriesValidator = listCategoriesValidator;
    }

    private readonly IValidator<ListCategoriesRequest> _listCategoriesValidator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]

    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var category = await _createCategoryUseCase.ExecuteAsync(request);

        if (category.IsFailed)
        {
            return FromErrors(category.Errors);
        }
        return Ok(category.Value);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] ListCategoriesRequest request)
    {
        var validationResult = await _listCategoriesValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FromValidationErrors(validationResult.Errors);
        }

        var categories = await _listCategoriesUseCase.ExecuteAsync(request);
        return Ok(categories);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequest request)
    {
        var result = await _updateCategoryUseCase.ExecuteAsync(id, request);

        if (result.IsFailed)
        {
            return FromErrors(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var result = await _deleteCategoryUseCase.ExecuteAsync(id);
        if (result.IsFailed)
        {
            return FromErrors(result.Errors);
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
            return FromErrors(result.Errors);
        }

        return NoContent();
    }

}