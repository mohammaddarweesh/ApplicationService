using StorageService.ViewModels;

namespace StorageService.Services.IServices
{
    public interface IStorageService
    {
        void StoreUploadMetadata(string uploadId, FileMetadata metadata, string signature, DateTime expiresAt);
        UploadMetadata GetUploadMetadata(string uploadId);
        Task StoreFileAsync(string fileId, Stream fileStream);
        bool FileExists(string fileId);
    }
}
