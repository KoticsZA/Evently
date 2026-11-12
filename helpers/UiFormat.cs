using System.Globalization;

namespace Evently.Helpers;

public static class UiFormat
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static string Rand(decimal amount) => "R" + amount.ToString("N2", Inv);

    public static string Day(DateTime d) => d.ToString("ddd, dd MMM", Inv);
    public static string LongDay(DateTime d) => d.ToString("ddd, d MMMM yyyy", Inv);
    public static string Time(DateTime d) => d.ToString("HH:mm", Inv);
    public static string ShortDate(DateTime d) => d.ToString("d MMM", Inv);

    public static string Greeting(DateTime now) =>
        now.Hour < 12 ? "Good morning" : now.Hour < 18 ? "Good afternoon" : "Good evening";

    public static string DaysUntil(DateTime start)
    {
        var days = (start.Date - DateTime.Today).Days;
        return days < 0 ? "Past event" : days == 0 ? "Today" : days == 1 ? "Tomorrow" : $"In {days} days";
    }

    public static string Ago(DateTime when)
    {
        var days = (DateTime.Today - when.Date).Days;
        return days <= 0 ? "Today, " + Time(when) : days == 1 ? "Yesterday" : ShortDate(when);
    }

    public static string MapsUrl(string query) =>
        "https://www.google.com/maps/search/?api=1&query=" + Uri.EscapeDataString(query);

    public static string GoogleCalendarUrl(string title, DateTime start, DateTime end, string location)
    {
        if (end <= start) end = start.AddHours(3);
        return "https://calendar.google.com/calendar/render?action=TEMPLATE"
             + "&text=" + Uri.EscapeDataString(title)
             + "&dates=" + start.ToString("yyyyMMdd'T'HHmmss", Inv) + "/" + end.ToString("yyyyMMdd'T'HHmmss", Inv)
             + "&location=" + Uri.EscapeDataString(location);
    }
}
