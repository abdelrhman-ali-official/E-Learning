namespace Services.Abstractions
{
    public interface IStorageService
    {
        /// <summary>
        /// Upload file to storage and return the public URL
        /// </summary>
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);

        /// <summary>
        /// Delete file from storage
        /// </summary>
        Task DeleteFileAsync(string fileUrl);

        /// <summary>
        /// Validate file type
        /// </summary>
        bool IsValidImageType(string contentType);

        /// <summary>
        /// Validate file size
        /// </summary>
        bool IsValidFileSize(long fileSize);

        /// <summary>
        /// Upload video to storage and return the object key
        /// </summary>
        Task<string> UploadVideoAsync(Stream fileStream, string fileName, string contentType);

        /// <summary>
        /// Generate pre-signed URL for video streaming (short-lived)
        /// </summary>
        Task<string> GenerateVideoStreamUrlAsync(string objectKey, int expirationMinutes = 10);

        /// <summary>
        /// Validate video file type
        /// </summary>
        bool IsValidVideoType(string contentType);
    }
}
