using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VrtNuDownloader.Core.Models;
using VrtNuDownloader.Core.Service.Ffmpeg;
using VrtNuDownloader.Core.Service.File;

namespace VrtNuDownloader.Core.Extensions
{
    public static class EpisodeInfoExtensions
    {
        public static bool DownloadToFolder(this EpisodeInfo episodeInfo)
        {
            IFileService fileService = Context.Context.Container.GetRequiredService<IFileService>();
            Configuration configuration = Context.Context.Container.GetRequiredService<IOptions<Configuration>>().Value;
            IFfmpegService ffmpegService = Context.Context.Container.GetRequiredService<IFfmpegService>();

            var filename = fileService.MakeValidFileName(episodeInfo.GetFileName());

            var savePathBuilder = new SavePathBuilder(fileService)
                .ForEpisode(episodeInfo)
                .SetBasePath(configuration.SavePath);

            if (configuration.SaveShowsInFolders)
                savePathBuilder.AddShowFolder();
            if (configuration.SaveSeasonsInFolders)
                savePathBuilder.AddSeasonFolder();

            var savePath = savePathBuilder.Build();

            fileService.EnsureFolderExists(configuration.DownloadPath);
            fileService.EnsureFolderExists(savePath);

            return ffmpegService.DownloadAndMoveEpisode(episodeInfo.StreamUrl, filename, configuration.DownloadPath, savePath);
        }

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
            return filename + ".mp4";
        }
    }
}
