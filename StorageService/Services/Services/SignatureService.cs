using StorageService.Services.IServices;
using StorageService.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace StorageService.Services.Services
{
    public class SignatureService : ISignatureService
    {
        private readonly string _secretKey;

        public SignatureService(IConfiguration configuration)
        {
            _secretKey = configuration["ApiKey"] ?? "default-secret-key";
        }

        public string GenerateSignature(FileMetadata metadata, string uploadId)
        {
            // Create a signature string based on file metadata and uploadId
            // The format can be anything consistent, here we use a simple concatenation
            string dataToSign = $"{uploadId}:{metadata.FileName}:{metadata.FileSize}:{metadata.MimeType}:{_secretKey}";

            // Create a SHA256 hash of the data
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(dataToSign);
                byte[] hash = sha256.ComputeHash(bytes);

                // Convert the hash to a Base64 string for URL-friendly signature
                return Convert.ToBase64String(hash)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .Replace("=", "");
            }
        }
    }
}
