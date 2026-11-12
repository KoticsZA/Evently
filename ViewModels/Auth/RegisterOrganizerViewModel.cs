using System.ComponentModel.DataAnnotations;

namespace Evently.ViewModels.Auth
{
    public class RegisterOrganizerViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "Organisation name is required.")]
        [StringLength(120)]
        [Display(Name = "Organisation name")]
        public string OrganizerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tell attendees a little about the events you host.")]
        [StringLength(1000, MinimumLength = 20, ErrorMessage = "Please write at least 20 characters.")]
        [Display(Name = "About your organisation")]
        public string Description { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Public contact email")]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Contact number")]
        public string? ContactNumber { get; set; }

        [Url(ErrorMessage = "Please enter a full website address, e.g. https://example.co.za")]
        [Display(Name = "Website")]
        public string? WebsiteUrl { get; set; }
        public bool AgreeTerms { get; set; } = false;
    }
    
}
