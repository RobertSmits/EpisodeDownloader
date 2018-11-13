namespace VrtNuDownloader.Downloader.Vier.Models.Auth
{
    public class RefreshRequestPayload
    {
        public RefreshRequestPayload(string clientId, string refreshToken)
        {
            ClientId = clientId;
            AuthParameters = new Authparameters() { REFRESH_TOKEN = refreshToken };
        }

        public string ClientId { get; set; }
        public string AuthFlow { get; set; } = "REFRESH_TOKEN_AUTH";
        public Authparameters AuthParameters { get; set; }
    }

    public class Authparameters
    {
        public string REFRESH_TOKEN { get; set; }
    }
}
