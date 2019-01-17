namespace EpisodeDownloader.Downloader.Vier.Models.Auth
{
    public class Authentication
    {
        public AuthenticationResult AuthenticationResult { get; set; }
        public Challengeparameters ChallengeParameters { get; set; }
    }

    public class AuthenticationResult
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string IdToken { get; set; }
        public string TokenType { get; set; }
    }

    public class Challengeparameters
    {
    }

}
