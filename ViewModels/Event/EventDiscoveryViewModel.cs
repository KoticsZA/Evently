namespace Evently.ViewModels.Events
{
    public class EventDiscoveryViewModel
    {
        public EventSearchFilter Filter { get; set; } = new();

        public ICollection<EventCardViewModel> Events { get; set; } = new List<EventCardViewModel>();

        public ICollection<EventCardViewModel> OtherRegionEvents { get; set; } = new List<EventCardViewModel>();

        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public IReadOnlyList<FilterOption> Categories { get; set; } = Array.Empty<FilterOption>();

        public IReadOnlyList<FilterOption> Regions { get; set; } = new[]
        {
        new FilterOption("",    "All South Africa"),
        new FilterOption("CPT", "Cape Town & Winelands"),
        new FilterOption("JHB", "Johannesburg"),
        new FilterOption("DUR", "Durban"),
        new FilterOption("PTA", "Pretoria"),
        new FilterOption("PLZ", "Gqeberha")
    };

        public IReadOnlyList<FilterOption> Dates { get; set; } = new[]
        {
        new FilterOption("",        "Any date"),
        new FilterOption("weekend", "This weekend"),
        new FilterOption("month",   "This month"),
        new FilterOption("quarter", "Next 3 months")
    };

        public IReadOnlyList<FilterOption> Prices { get; set; } = new[]
        {
        new FilterOption("",         "Any price"),
        new FilterOption("free",     "Free"),
        new FilterOption("under200", "Under R200"),
        new FilterOption("200to500", "R200 – R500"),
        new FilterOption("500plus",  "R500+")
    };

        public IReadOnlyList<FilterOption> SortOptions { get; set; } = new[]
        {
        new FilterOption("",           "Recommended"),
        new FilterOption("soonest",    "Soonest first"),
        new FilterOption("price_low",  "Price: low to high"),
        new FilterOption("price_high", "Price: high to low")
    };
    }
}