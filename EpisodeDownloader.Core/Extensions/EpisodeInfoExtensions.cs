using EpisodeDownloader.Contracts;

namespace EpisodeDownloader.Core.Extensions
{
    public static class EpisodeInfoExtensions
    {
        public static string GetFileName(this EpisodeInfo episodeInfo)
        {
            var filename = episodeInfo.ShowName;
            if (int.TryParse(episodeInfo.Season, out int seasonNr))
            {
                var episodeNumber = episodeInfo.Episode < 100 ? episodeInfo.Episode.ToString("00") : episodeInfo.Episode.ToString("000");
                filename += ($" - S{seasonNr:00}E{episodeNumber}");
            }
            else
            {
                filename += ($" - S{seasonNr:00}E{episodeInfo.Episode}");
            }
            if (!string.IsNullOrWhiteSpace(episodeInfo.Title))
            {
                filename += $" - {episodeInfo.Title}";
            }
            return filename;
        }
    }
}
