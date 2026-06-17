using BetterCRM.Core.Interfaces.Services;
using Minio;
using Minio.DataModel.Args;

namespace BetterCRM.Business.Services
{
    public class MinioStorageService : IFileStorageService
    {
        private readonly IMinioClient _minio;

        public MinioStorageService(IMinioClient minio) => _minio = minio;

        public async Task DeleteAsync(string bucket, string objectName)
        {
            await _minio.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucket).WithObject(objectName));
        }

        public async Task<Stream> DownloadAsync(string bucket, string objectName)
        {
            var ms = new MemoryStream();

            await _minio.GetObjectAsync(new GetObjectArgs().WithBucket(bucket).WithObject(objectName).WithCallbackStream(stream => stream.CopyTo(ms)));
            ms.Position = 0;
            return ms;
        }

        public async Task EnsureBucketExistsAsync(string bucket)
        {
            var exists = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket));
            if (!exists) await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket));
        }

        public async Task<string> GetPresignedUrlAsync(string bucket, string objectName, int expirySeconds = 3600)
        {
            return await _minio.PresignedGetObjectAsync(new PresignedGetObjectArgs().WithBucket(bucket).WithObject(objectName).WithExpiry(expirySeconds));
        }

        public async Task UploadAsync(string bucket, string objectName, Stream stream, string contentType)
        {
            await EnsureBucketExistsAsync(bucket);

            await _minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType));
        }
    }
}
