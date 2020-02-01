namespace EpisodeDownloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var downloader = Bootstrapper.BuildEpisodeDownloader();
            downloader.Run();
        }
    }
}
