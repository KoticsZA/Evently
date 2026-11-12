namespace Evently.DTOs
{
    public class RegisterOrganizerDto : RegisterDto
    {
        public string OrganizerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactNumber { get; set; }
        public string? WebsiteUrl { get; set; }
    }
}
