using Microsoft.AspNetCore.Identity;

namespace Evently.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public Wallet? Wallet { get; set; }
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;

        public ICollection<EventOrganizer> OrganizedProfiles { get; set; }
            = new List<EventOrganizer>();

        public ICollection<TicketOrder> Orders { get; set; }
            = new List<TicketOrder>();
    }
}
