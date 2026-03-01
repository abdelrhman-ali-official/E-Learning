using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Services.Abstractions;

namespace Services
{
    public class CloudflareR2StorageService : IStorageService
    {
        private readonly AmazonS3Client _s3Client;
        private readonly string _bucketName;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB for images
        private const long MaxVideoSize = 500 * 1024 * 1024; // 500 MB for videos
        private readonly string[] _allowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };
        private readonly string[] _allowedVideoTypes = { "video/mp4" };

        public CloudflareR2StorageService(IConfiguration configuration)
        {
            var accessKey = configuration["CloudflareR2:AccessKey"] 
                ?? throw new InvalidOperationException("Cloudflare R2 Access Key not configured");
            
            var secretKey = configuration["CloudflareR2:SecretKey"] 
                ?? throw new InvalidOperationException("Cloudflare R2 Secret Key not configured");
            
            _bucketName = configuration["CloudflareR2:BucketName"] 
                ?? throw new InvalidOperationException("Cloudflare R2 Bucket Name not configured");
            
            var endpoint = configuration["CloudflareR2:Endpoint"] 
                ?? throw new InvalidOperationException("Cloudflare R2 Endpoint not configured");

            // Strip trailing slash — Cloudflare R2 rejects endpoints with trailing slashes
            endpoint = endpoint.TrimEnd('/');

            var config = new AmazonS3Config
            {
                ServiceURL      = endpoint,
                ForcePathStyle  = true,
                UseHttp         = false
            };

            _s3Client = new AmazonS3Client(accessKey, secretKey, config);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            if (!IsValidImageType(contentType))
                throw new InvalidOperationException($"Invalid file type: {contentType}. Only images are allowed.");

            if (!IsValidFileSize(fileStream.Length))
                throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024} MB.");

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var key = $"uploads/{DateTime.UtcNow:yyyy/MM}/{uniqueFileName}";

            try
            {
               
                var putRequest = new PutObjectRequest
                {
                    InputStream        = fileStream,
                    Key                = key,
                    BucketName         = _bucketName,
                    ContentType        = contentType,
                    CannedACL          = S3CannedACL.Private,
                    UseChunkEncoding   = false,
                    DisablePayloadSigning = true
                };

                await _s3Client.PutObjectAsync(putRequest);

                var presignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key        = key,
                    Expires    = DateTime.UtcNow.AddDays(7),
                    Protocol   = Protocol.HTTPS
                };

                return _s3Client.GetPreSignedURL(presignedUrlRequest);
            }
            catch (AmazonS3Exception ex)
            {
                throw new InvalidOperationException($"Error uploading file to Cloudflare R2: {ex.Message}", ex);
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                var key = uri.AbsolutePath.TrimStart('/').Replace($"{_bucketName}/", "");

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
            }
            catch (AmazonS3Exception ex)
            {
                throw new InvalidOperationException($"Error deleting file from Cloudflare R2: {ex.Message}", ex);
            }
        }

        public bool IsValidImageType(string contentType)
        {
            return _allowedImageTypes.Contains(contentType.ToLower());
        }

        public bool IsValidFileSize(long fileSize)
        {
            return fileSize > 0 && fileSize <= MaxFileSize;
        }

        public async Task<string> UploadVideoAsync(Stream fileStream, string fileName, string contentType)
        {
            if (!IsValidVideoType(contentType))
                throw new InvalidOperationException($"Invalid file type: {contentType}. Only MP4 videos are allowed.");

            if (!IsValidVideoSize(fileStream.Length))
                throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxVideoSize / 1024 / 1024} MB.");

            // Generate unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var key = $"videos/{DateTime.UtcNow:yyyy/MM}/{uniqueFileName}";

            try
            {
                // Cloudflare R2 does not support STREAMING-AWS4-HMAC-SHA256-PAYLOAD-TRAILER.
                var putRequest = new PutObjectRequest
                {
                    InputStream        = fileStream,
                    Key                = key,
                    BucketName         = _bucketName,
                    ContentType        = contentType,
                    CannedACL          = S3CannedACL.Private,
                    UseChunkEncoding   = false,
                    DisablePayloadSigning = true
                };

                await _s3Client.PutObjectAsync(putRequest);

                // Return only the object key, NOT a pre-signed URL
                return key;
            }
            catch (AmazonS3Exception ex)
            {
                throw new InvalidOperationException($"Error uploading video to Cloudflare R2: {ex.Message}", ex);
            }
        }

        public Task<string> GenerateVideoStreamUrlAsync(string objectKey, int expirationMinutes = 10)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key cannot be null or empty", nameof(objectKey));

            if (expirationMinutes < 1 || expirationMinutes > 60)
                throw new ArgumentException("Expiration minutes must be between 1 and 60", nameof(expirationMinutes));

            try
            {
                var presignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey,
                    Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                    Protocol = Protocol.HTTPS
                };

                var url = _s3Client.GetPreSignedURL(presignedUrlRequest);
                return Task.FromResult(url);
            }
            catch (AmazonS3Exception ex)
            {
                throw new InvalidOperationException($"Error generating video stream URL: {ex.Message}", ex);
            }
        }

        public bool IsValidVideoType(string contentType)
        {
            return _allowedVideoTypes.Contains(contentType.ToLower());
        }

        private bool IsValidVideoSize(long fileSize)
        {
            return fileSize > 0 && fileSize <= MaxVideoSize;
        }
    }
}
