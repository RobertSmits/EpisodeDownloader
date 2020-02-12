using System.Threading;
using System.Threading.Tasks;

namespace EpisodeDownloader
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var downloader = Bootstrapper.BuildEpisodeDownloader();
            return downloader.Run(new CancellationToken());
        }
    }
}
