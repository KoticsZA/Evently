
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Evently.ViewModels.Events
{
    public class CreateEventViewModel
    {
        [Required]
        [StringLength(150, MinimumLength = 5)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(3000, MinimumLength = 20)]
        public string Description { get; set; } = string.Empty;

        [ImageFile]                                   // see ImageFileAttribute.cs
        [Display(Name = "Cover image")]
        public IFormFile? CoverImage { get; set; }

        [Required]
        [StringLength(150)]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string StreetAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Province { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = "South Africa";

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> Provinces { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
