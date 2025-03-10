namespace AppService.ViewModels
{
    public class SignedUrlResponse
    {
        public string UploadUrl { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
