using AppService.Services.IService;
using AppService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IStorageServiceClient _storageClient;

        public UploadController(IStorageServiceClient storageClient)
        {
            _storageClient = storageClient;
        }

        [HttpPost("getSignedUrl")]
        public async Task<ActionResult<SignedUrlResponse>> GetSignedUrl([FromBody] FileMetadata metadata)
        {
            // Validate input
            if (metadata == null || string.IsNullOrEmpty(metadata.FileName) ||
                metadata.FileSize <= 0 || string.IsNullOrEmpty(metadata.MimeType))
            {
                return BadRequest("Invalid file metadata");
            }

            // Call the storage service to get a pre-signed URL
            var response = await _storageClient.GetSignedUrlAsync(metadata);

            return Ok(response);
        }
    }
}
