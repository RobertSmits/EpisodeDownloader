using System.Threading.Tasks;

namespace EpisodeDownloader.Downloader.Vier.Service
{
    public interface IVierAuthService
    {
        Task<string> GetIdTokenAsync();
    }
}
