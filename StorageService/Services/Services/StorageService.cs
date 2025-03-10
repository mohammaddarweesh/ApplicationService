using StorageService.Services.IServices;
using StorageService.ViewModels;
using System.Collections.Concurrent;

namespace StorageService.Services.Services
{
    public class StorageService : IStorageService
    {
        private readonly string _storageDirectory;
        private readonly ConcurrentDictionary<string, UploadMetadata> _uploadMetadata;

        public StorageService(IConfiguration configuration)
        {
            _storageDirectory = configuration["Storage:Directory"] ?? Path.Combine(Path.GetTempPath(), "FileStorage");
            _uploadMetadata = new ConcurrentDictionary<string, UploadMetadata>();

            // Create the storage directory if it doesn't exist
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }
        }

        public void StoreUploadMetadata(string uploadId, FileMetadata metadata, string signature, DateTime expiresAt)
        {
            _uploadMetadata[uploadId] = new UploadMetadata
            {
                FileMetadata = metadata,
                Signature = signature,
                ExpiresAt = expiresAt
            };
        }

        public UploadMetadata GetUploadMetadata(string uploadId)
        {
            _uploadMetadata.TryGetValue(uploadId, out var metadata);
            return metadata;
        }

        public async Task StoreFileAsync(string fileId, Stream fileStream)
        {
            string filePath = Path.Combine(_storageDirectory, fileId);

            using (var fileStream2 = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fileStream2);
            }
        }

        public bool FileExists(string fileId)
        {
            string filePath = Path.Combine(_storageDirectory, fileId);
            return File.Exists(filePath);
        }
    }
}
