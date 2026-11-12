using Evently.DTOs.Dashboard;

namespace Evently.DTOs.Events
{
    public class EventDiscoveryDto
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public List<EventCardDto> LocalEvents { get; set; }
            = new();

        public List<EventCardDto> NationalEvents { get; set; }
            = new();
    }
}