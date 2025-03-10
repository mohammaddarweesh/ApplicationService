using AppService.Services.IService;
using AppService.ViewModels;

namespace AppService.Services.Service
{
    public class StorageServiceClient : IStorageServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _storageServiceBaseUrl;
        private readonly string _secretKey;

        public StorageServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _storageServiceBaseUrl = configuration["StorageService:BaseUrl"];
            _secretKey = configuration["StorageService:SecretKey"];
        }

        public async Task<SignedUrlResponse> GetSignedUrlAsync(FileMetadata metadata)
        {
            // Add the secret key to the request header
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_storageServiceBaseUrl}/storage/signedUrl")
            {
                Content = JsonContent.Create(metadata)
            };
            request.Headers.Add("X-API-Key", _secretKey);

            var response = await _httpClient.SendAsync(request);


            //var response = await _httpClient.PostAsJsonAsync($"{_storageServiceBaseUrl}/storage/signedUrl", metadata);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<SignedUrlResponse>();
        }

        public async Task<bool> ValidateImageIdAsync(string imageId)
        {
            // Add the secret key to the request header
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _secretKey);

            var response = await _httpClient.GetAsync($"{_storageServiceBaseUrl}/storage/validate/{imageId}");

            return response.IsSuccessStatusCode;
        }
    }
}
