using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Evently.ViewModels.Events
{
    /// <summary>Field-level check so the error shows next to the upload. The storage service still verifies the real file contents.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ImageFileAttribute : ValidationAttribute
    {
        private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly long _maxBytes;

        public ImageFileAttribute(int maxMegabytes = 4)
        {
            _maxBytes = maxMegabytes * 1024L * 1024L;
            ErrorMessage = $"Choose a JPG, PNG or WebP image up to {maxMegabytes} MB.";
        }

        public override bool IsValid(object? value)
        {
            if (value is null) return true;                      
            if (value is not IFormFile file) return false;
            if (file.Length == 0) return true;                    
            if (file.Length > _maxBytes) return false;

            return Allowed.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());
        }
    }
}