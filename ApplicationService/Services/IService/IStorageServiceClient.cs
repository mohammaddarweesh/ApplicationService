using AppService.ViewModels;

namespace AppService.Services.IService
{
    public interface IStorageServiceClient
    {
        Task<SignedUrlResponse> GetSignedUrlAsync(FileMetadata metadata);
        Task<bool> ValidateImageIdAsync(string imageId);
    }
}
