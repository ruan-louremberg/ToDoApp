using System.Text.RegularExpressions;
using FluentResults;
using ToDoApp.Application.DTO;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class CreateCategoryUseCase
{
    // Regex compilada para evitar recompilação a cada execução
    private static readonly Regex HexColorRegex = new(@"^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);
    
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<Category>> ExecuteAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length < 2 || request.Name.Trim().Length > 50)
        {
            return Result.Fail("O nome da categoria deve ter entre 2 e 50 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(request.Color) || !HexColorRegex.IsMatch(request.Color))
        {
            return Result.Fail("A cor informada é inválida. Informe um código hexadecimal no formato #RRGGBB (ex: #FF5733).");
        }

        var existingCategory = await _categoryRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingCategory != null)
        {
            return Result.Fail("Categoria já existe.");
        }

        var category = new Category(request.Name, request.Color);
        await _categoryRepository.AddAsync(category, cancellationToken);

        return Result.Ok(category);
    }
}