using FluentResults;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class RestoreCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public RestoreCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

   public async Task<Result> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        var categoria = await _categoryRepository.GetDeletedByIdAsync(id, cancellationToken);

        if (categoria is null)
        {
            return Result.Fail("Categoria não encontrada na lixeira.");
        }

        var existingCategory = await _categoryRepository.GetByNameAsync(categoria.Name, cancellationToken);

        if (existingCategory != null)
        {
            return Result.Fail("Já existe uma categoria com o mesmo nome.");
        }

        var restored = await _categoryRepository.RestoreAsync(
            id,
            cancellationToken);

        if (!restored)
        {
            return Result.Fail("Categoria não encontrada na lixeira.");
        }

        return Result.Ok();
    }
}