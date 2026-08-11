namespace razor_exercise.Models;

public class PagedResult<T>
{
    public List<T> Items { get; init; } = [];

    public int PageNumber { get; init; }

    public int TotalPages { get; init; }

    public int TotalCount { get; init; }
}
