using ToDoApp.Application.DTO;
using ToDoApp.Domain.Interfaces.Repositories;

namespace ToDoApp.Application.UseCases;

public class ListCategoriesUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public ListCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ListCategoriesResponse> ExecuteAsync(ListCategoriesRequest request, CancellationToken cancellationToken = default)
    {
        var safePage = Math.Max(request.Page, 1);
        var safePageSize = Math.Clamp(request.PageSize, 1, 100);

        var categories = await _categoryRepository.GetAllAsync(
            request.Name,
            request.Color,
            safePage,
            safePageSize,
            cancellationToken
        );

        var totalItems = await _categoryRepository.CountAsync(
            request.Name,
            request.Color,
            cancellationToken
        );
        var totalPages = (int)Math.Ceiling((double)totalItems / safePageSize);
        return new ListCategoriesResponse
        {
            Items = categories.Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.Color
            )).ToList(),
            Page = safePage,
            PageSize = safePageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }
}