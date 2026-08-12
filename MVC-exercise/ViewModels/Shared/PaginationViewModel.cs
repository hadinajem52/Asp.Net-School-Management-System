namespace MVC_exercise.ViewModels.Shared;

public class PaginationViewModel
{
    public required string AriaLabel { get; init; }

    public required string Controller { get; init; }

    public string Action { get; init; } = "Index";

    public int PageNumber { get; init; }

    public int TotalPages { get; init; }

    public string? SearchTerm { get; init; }

    public string? SortBy { get; init; }
}
