namespace BetterCRM.Core.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task UploadAsync(string bucket, string objectName, Stream stream, string contentType);
        Task DeleteAsync(string bucket, string objectName);
        Task<Stream> DownloadAsync(string bucket, string objectName);
        Task<string> GetPresignedUrlAsync(string bucket, string objectName, int expirySeconds = 3600);
        Task EnsureBucketExistsAsync(string bucket);
    }
}
