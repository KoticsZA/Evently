using Evently.DTOs;

namespace Evently.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> SaveAsync(ImageUploadDto upload, string folder, CancellationToken cancellationToken = default);
        Task DeleteAsync(string? url, CancellationToken cancellationToken = default);
    }
}
