using Microsoft.AspNetCore.Mvc;

namespace Evently.ViewModels.Events;

public record FilterOption(string Value, string Label, string? Icon = null);

public record FilterSelectModel(string Name, string Label, string? Selected, IReadOnlyList<FilterOption> Options);

public class EventSearchFilter
{
    [FromQuery(Name = "q")] public string? Query { get; set; }
    [FromQuery(Name = "category")] public string? Category { get; set; }
    [FromQuery(Name = "region")] public string? Region { get; set; }
    [FromQuery(Name = "when")] public string? When { get; set; }
    [FromQuery(Name = "price")] public string? Price { get; set; }
    [FromQuery(Name = "sort")] public string? Sort { get; set; }
    [FromQuery(Name = "page")] public int Page { get; set; } = 1;

    public bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(Query) || !string.IsNullOrWhiteSpace(Category) ||
        !string.IsNullOrWhiteSpace(Region) || !string.IsNullOrWhiteSpace(When) ||
        !string.IsNullOrWhiteSpace(Price);
}

