namespace VrtNuDownloader.Downloader.Vier.Models.Auth
{
    public class CredentialResult
    {
        public Credentials Credentials { get; set; }
        public string IdentityId { get; set; }
    }

    public class Credentials
    {
        public string AccessKeyId { get; set; }
        public float Expiration { get; set; }
        public string SecretKey { get; set; }
        public string SessionToken { get; set; }
    }
}
