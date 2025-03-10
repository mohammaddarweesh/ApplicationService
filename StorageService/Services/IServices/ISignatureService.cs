using StorageService.ViewModels;

namespace StorageService.Services.IServices
{
    public interface ISignatureService
    {
        string GenerateSignature(FileMetadata metadata, string uploadId);
    }
}
