using Microsoft.AspNetCore.Mvc;
using StorageService.Services.IServices;
using StorageService.ViewModels;

namespace StorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly ISignatureService _signatureService;
        private readonly IConfiguration _configuration;

        public StorageController(
            IStorageService storageService,
            ISignatureService signatureService,
            IConfiguration configuration)
        {
            _storageService = storageService;
            _signatureService = signatureService;
            _configuration = configuration;
        }

        [HttpPost("signedUrl")]
        public ActionResult<SignedUrlResponse> GetSignedUrl([FromBody] FileMetadata metadata, [FromHeader(Name = "X-API-Key")] string apiKey)
        {
            // Validate API key
            if (string.IsNullOrEmpty(apiKey) || apiKey != _configuration["ApiKey"])
            {
                return Unauthorized("Invalid API key");
            }

            // Validate input
            if (metadata == null || string.IsNullOrEmpty(metadata.FileName) ||
                metadata.FileSize <= 0 || string.IsNullOrEmpty(metadata.MimeType))
            {
                return BadRequest("Invalid file metadata");
            }

            // Generate a unique identifier for this upload
            string uploadId = Guid.NewGuid().ToString();

            // Generate a signature for the upload
            string signature = _signatureService.GenerateSignature(metadata, uploadId);

            // Storage Public Url
            string url = _configuration["Storage:Url"];

            // Create the pre-signed URL (in a real system, this would be an Amazon S3 or Azure Storage URL)
            string uploadUrl = $"{Request.Scheme}://{url}/api/storage/upload/{uploadId}?signature={signature}";

            // Set expiration time (e.g., 15 minutes from now)
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(15);

            // Store the metadata and signature for validation when the file is uploaded
            _storageService.StoreUploadMetadata(uploadId, metadata, signature, expiresAt);

            return Ok(new SignedUrlResponse
            {
                UploadUrl = uploadUrl,
                ExpiresAt = expiresAt
            });
        }

        [HttpPost("upload/{uploadId}")]
        public async Task<ActionResult<UploadResponse>> UploadFile(string uploadId, [FromQuery] string signature)
        {
            // Validate the upload ID and signature
            var metadata = _storageService.GetUploadMetadata(uploadId);

            if (metadata == null)
            {
                return BadRequest("Invalid upload ID");
            }

            if (metadata.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("Upload URL has expired");
            }

            if (metadata.Signature != signature)
            {
                return BadRequest("Invalid signature");
            }

            // Get the file from the request
            if (Request.Form.Files.Count == 0)
            {
                return BadRequest("No file uploaded");
            }

            var file = Request.Form.Files[0];

            // Validate the file matches the metadata
            //if (file.Length != metadata.FileMetadata.FileSize)
            //{
            //    return BadRequest("File size does not match");
            //}

            //if (file.ContentType != metadata.FileMetadata.MimeType)
            //{
            //    return BadRequest("File type does not match");
            //}

            // Store the file
            using (var stream = file.OpenReadStream())
            {
                await _storageService.StoreFileAsync(uploadId, stream);
            }

            // Return the file ID (which is the same as the upload ID in this case)
            return Ok(new UploadResponse
            {
                FileId = uploadId
            });
        }

        [HttpGet("validate/{fileId}")]
        public ActionResult ValidateFile(string fileId, [FromHeader(Name = "X-API-Key")] string apiKey)
        {
            // Validate API key
            if (string.IsNullOrEmpty(apiKey) || apiKey != _configuration["ApiKey"])
            {
                return Unauthorized("Invalid API key");
            }

            // Check if the file exists
            bool exists = _storageService.FileExists(fileId);

            if (!exists)
            {
                return NotFound("File not found");
            }

            return Ok();
        }

        [HttpGet("health")]
        public ActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
