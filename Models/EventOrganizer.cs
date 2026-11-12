namespace Evently.Models
{
    public class EventOrganizer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string OwnerUserId { get; set; } = string.Empty;
        public AppUser OwnerUser { get; set; } = null!;
        public ICollection<OrganizerEvent> Events { get; set; }= new List<OrganizerEvent>();
        public bool IsVerified { get; set; } = false;
    }
}
