using System.Text.RegularExpressions;
using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class UpdateCategoryUseCase
{
    private static readonly Regex HexColorRegex = new(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> ExecuteAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Name != null && (request.Name.Trim().Length < 2 || request.Name.Trim().Length > 50))
        {
            return Result.Fail("O nome da categoria deve ter entre 2 e 50 caracteres e não pode ser vazio.");
        }

        if (request.Color != null && !HexColorRegex.IsMatch(request.Color))
        {
            return Result.Fail("A cor informada é inválida. Informe um código hexadecimal no formato #RRGGBB (ex: #FF5733).");
        }
        
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

        if (category == null)
        {
            return Result.Fail("Categoria não encontrada.");
        }

        var existingCategory = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);

        if (existingCategory != null && existingCategory.Id != id)
        {
            return Result.Fail("Categoria já existe.");
        }

        category.UpdateCategory(request.Name, request.Color);

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        var categoryResponse = new CategoryResponse(
            category.Id,
            category.Name,
            category.Color
        );

        return Result.Ok(categoryResponse);
    }
}