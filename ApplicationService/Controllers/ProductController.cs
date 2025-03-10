using AppService.Services.IService;
using AppService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AppService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IStorageServiceClient _storageClient;

        public ProductController(IStorageServiceClient storageClient)
        {
            _storageClient = storageClient;
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            // Validate input
            if (request == null || string.IsNullOrEmpty(request.ImageId))
            {
                return BadRequest("Invalid product data");
            }

            // Validate the image ID exists in storage
            bool isValidImage = await _storageClient.ValidateImageIdAsync(request.ImageId);

            if (!isValidImage)
            {
                return BadRequest("Invalid image ID");
            }

            // In a real application, we would save the product to a database
            // For this example, we'll just return success
            return Ok(new { Message = "Product created successfully", ProductId = Guid.NewGuid().ToString() });
        }
    }
}
