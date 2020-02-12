using System.Threading.Tasks;

namespace EpisodeDownloader.Downloader.Vrt.Service
{
    public interface IVrtTokenService
    {
        Task<string> GetVrtTokenAsync();
        Task<string> GetPlayerTokenAsync();
    }
}
