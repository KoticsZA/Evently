using Evently.DTOs;
using Evently.Interfaces;

namespace Evently.Services
{
    
    public class LocalImageStorageService : IImageStorageService
    {
        public const long MaxBytes = 4 * 1024 * 1024;   
        private const string UploadsRoot = "uploads";

        private readonly string _webRoot;

        public LocalImageStorageService(IWebHostEnvironment env)
        {
            _webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        }

        public async Task<string> SaveAsync(ImageUploadDto upload, string folder, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(upload);

            if (string.IsNullOrWhiteSpace(folder) || Path.GetFileName(folder) != folder)
                throw new ArgumentException("Invalid upload folder.", nameof(folder));

            if (upload.Length <= 0)
                throw new ArgumentException("The selected image is empty.");

            if (upload.Length > MaxBytes)
                throw new ArgumentException("The cover image must be 4 MB or smaller.");

            var header = new byte[12];
            var headerLength = await ReadHeaderAsync(upload.Content, header, cancellationToken);
            var extension = DetectExtension(header.AsSpan(0, headerLength))
                ?? throw new ArgumentException("The cover image must be a JPG, PNG or WebP file.");

            var directory = Path.Combine(_webRoot, UploadsRoot, folder);
            Directory.CreateDirectory(directory);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var path = Path.Combine(directory, fileName);

            try
            {
                await using var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);

                await file.WriteAsync(header.AsMemory(0, headerLength), cancellationToken);

                long total = headerLength;
                var buffer = new byte[81920];
                int read;
                while ((read = await upload.Content.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    total += read;
                    if (total > MaxBytes)
                        throw new ArgumentException("The cover image must be 4 MB or smaller.");

                    await file.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                }
            }
            catch
            {
                TryDelete(path);
                throw;
            }

            return $"/{UploadsRoot}/{folder}/{fileName}";
        }

        public Task DeleteAsync(string? url, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith($"/{UploadsRoot}/", StringComparison.Ordinal))
                return Task.CompletedTask;

            var root = Path.GetFullPath(Path.Combine(_webRoot, UploadsRoot)) + Path.DirectorySeparatorChar;
            var full = Path.GetFullPath(Path.Combine(_webRoot, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));

            if (full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                TryDelete(full);

            return Task.CompletedTask;
        }

        private static async Task<int> ReadHeaderAsync(Stream stream, byte[] buffer, CancellationToken ct)
        {
            var total = 0;
            while (total < buffer.Length)
            {
                var n = await stream.ReadAsync(buffer.AsMemory(total), ct);
                if (n == 0) break;
                total += n;
            }
            return total;
        }

        private static string? DetectExtension(ReadOnlySpan<byte> h)
        {
            if (h.Length >= 3 && h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF)
                return ".jpg";

            if (h.Length >= 8 && h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47 &&
                h[4] == 0x0D && h[5] == 0x0A && h[6] == 0x1A && h[7] == 0x0A)
                return ".png";

            if (h.Length >= 12 && h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46 &&
                h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50)
                return ".webp";

            return null;
        }

        private static void TryDelete(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch (IOException) { /* best effort */ }
            catch (UnauthorizedAccessException) { /* best effort */ }
        }
    }
}