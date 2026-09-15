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
    private readonly UpdateCategoryUseCase _updateCategoryUseCase;

    public CategoryController(CreateCategoryUseCase createCategoryUseCase, ListCategoriesUseCase listCategoriesUseCase, UpdateCategoryUseCase updateCategoryUseCase)
    {
        _createCategoryUseCase = createCategoryUseCase;
        _listCategoriesUseCase = listCategoriesUseCase;
        _updateCategoryUseCase = updateCategoryUseCase;
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

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await _updateCategoryUseCase.ExecuteAsync(id, request);

        if(category.IsFailed)
        {
            if(category.Errors[0].Message == "Categoria não encontrada.")
            {
                return NotFound(category.Errors[0].Message);
            }
            else if(category.Errors[0].Message == "O nome da categoria deve ter entre 2 e 50 caracteres e não pode ser vazio.")
            {
                return BadRequest(category.Errors[0].Message);
            }
            else if(category.Errors[0].Message == "Categoria já existe.")
            {
                return Conflict(category.Errors[0].Message);
            }
        }
        return Ok(category.Value);
    }
}