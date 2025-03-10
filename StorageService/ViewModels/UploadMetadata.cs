namespace StorageService.ViewModels
{
    public class UploadMetadata
    {
        public FileMetadata FileMetadata { get; set; }
        public string Signature { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
