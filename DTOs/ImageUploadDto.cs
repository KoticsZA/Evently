namespace Evently.DTOs
{
    public sealed class ImageUploadDto : IAsyncDisposable
    {
        public string FileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public long Length { get; init; }
        public Stream Content { get; init; } = Stream.Null;

        public ValueTask DisposeAsync() => Content.DisposeAsync();
    }
}