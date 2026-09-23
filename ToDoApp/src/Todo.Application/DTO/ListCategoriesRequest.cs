namespace ToDoApp.Application.DTO;

public record ListCategoriesRequest
{
    public string? Name { get; set; } = null;

    public string? Color { get; set; } = null;

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}

public record ListCategoriesResponse
{
    public List<CategoryResponse> Items { get; set; } = [];

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }
}

