namespace Evently.Constants
{
    public static class SouthAfricaLocations
    {
        public static readonly IReadOnlyDictionary<string, string[]> CitiesByProvince =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["Eastern Cape"] = new[]
                {
                    "Gqeberha", "East London", "Mthatha", "Komani (Queenstown)", "Makhanda (Grahamstown)",
                    "Kariega (Uitenhage)", "Jeffreys Bay", "Port Alfred", "Graaff-Reinet", "Bhisho"
                },
                ["Free State"] = new[]
                {
                    "Bloemfontein", "Welkom", "Bethlehem", "Sasolburg", "Kroonstad", "Parys",
                    "Harrismith", "Phuthaditjhaba", "Virginia"
                },
                ["Gauteng"] = new[]
                {
                    "Johannesburg", "Pretoria", "Sandton", "Soweto", "Centurion", "Midrand", "Boksburg",
                    "Benoni", "Germiston", "Springs", "Roodepoort", "Randburg", "Krugersdorp", "Vereeniging",
                    "Vanderbijlpark", "Kempton Park", "Alberton", "Brakpan"
                },
                ["KwaZulu-Natal"] = new[]
                {
                    "Durban", "Pietermaritzburg", "Ladysmith", "Richards Bay", "Newcastle", "Umhlanga", "Ballito",
                    "Pinetown", "Port Shepstone", "Margate", "Empangeni", "Vryheid", "Ulundi", "KwaDukuza (Stanger)"
                },
                ["Limpopo"] = new[]
                {
                    "Polokwane", "Mokopane", "Tzaneen", "Thohoyandou", "Phalaborwa", "Musina",
                    "Lephalale", "Bela-Bela", "Makhado (Louis Trichardt)"
                },
                ["Mpumalanga"] = new[]
                {
                    "Mbombela (Nelspruit)", "eMalahleni (Witbank)", "Middelburg", "Secunda", "Ermelo",
                    "Standerton", "Hazyview", "Sabie", "Barberton"
                },
                ["North West"] = new[]
                {
                    "Rustenburg", "Mahikeng", "Klerksdorp", "Potchefstroom", "Brits", "Vryburg", "Lichtenburg", "Mogwase"
                },
                ["Northern Cape"] = new[]
                {
                    "Kimberley", "Upington", "Springbok", "De Aar", "Kuruman", "Kathu", "Postmasburg", "Colesberg"
                },
                ["Western Cape"] = new[]
                {
                    "Cape Town", "Stellenbosch", "Paarl", "Franschhoek", "George", "Mossel Bay", "Knysna",
                    "Worcester", "Hermanus", "Oudtshoorn", "Somerset West", "Bellville", "Swellendam",
                    "Plettenberg Bay", "Saldanha"
                }
            };

        public static IReadOnlyList<string> Provinces { get; } = CitiesByProvince.Keys.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();

        public static IReadOnlyList<string> CitiesFor(string? province) =>
            !string.IsNullOrWhiteSpace(province) && CitiesByProvince.TryGetValue(province.Trim(), out var cities)
                ? cities
                : Array.Empty<string>();

        public static bool IsValidProvince(string? province) =>
            !string.IsNullOrWhiteSpace(province) && CitiesByProvince.ContainsKey(province.Trim());

        public static bool IsValidCity(string? province, string? city) =>
            !string.IsNullOrWhiteSpace(city) &&
            CitiesFor(province).Contains(city.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
