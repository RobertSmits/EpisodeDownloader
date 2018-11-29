using VrtNuDownloader.Downloader.Vier.Models.Auth;

namespace VrtNuDownloader.Downloader.Vier.Service
{
    public interface IVierAuthService
    {
        string IdToken { get; }
        string AccessToken { get; }
    }
}
