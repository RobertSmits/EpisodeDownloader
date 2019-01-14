using System;
using VrtNuDownloader.Core.Service.Config;
using VrtNuDownloader.Core.Service.Ffmpeg;
using VrtNuDownloader.Core.Service.File;

namespace VrtNuDownloader.Core.Models
{
    public class EpisodeInfo
    {
        public string ShowName { get; set; }
        public string Season { get; set; }
        public int Episode { get; set; }
        public string Title { get; set; }

        public Uri StreamUrl { get; set; }

        public bool DownloadToFolder(IFileService fileService, IConfigService configService, IFfmpegService ffmpegService)
        {
            var filename = fileService.MakeValidFileName(GetFileName());

            var savePathBuilder = new SavePathBuilder(fileService)
                .ForEpisode(this)
                .SetBasePath(configService.SavePath);

            if (configService.SaveShowsInFolders)
                savePathBuilder.AddShowFolder();
            if (configService.SaveSeasonsInFolders)
                savePathBuilder.AddSeasonFolder();

            var savePath = savePathBuilder.Build();

            fileService.EnsureFolderExists(configService.DownloadPath);
            fileService.EnsureFolderExists(savePath);

            return ffmpegService.DownloadAndMoveEpisode(StreamUrl, filename, configService.DownloadPath, savePath);
        }

        public string GetFileName()
        {
            var filename = ShowName;
            if (int.TryParse(Season, out int seasonNr))
            {
                var episodeNumber = Episode < 100 ? Episode.ToString("00") : Episode.ToString("000");
                filename += ($" - S{seasonNr:00}E{episodeNumber}");
            }
            else
            {
                filename += ($" - S{seasonNr:00}E{Episode}");
            }
            if (!string.IsNullOrWhiteSpace(Title)) {
                filename += $" - {Title}";
            }
            return filename + ".mp4";
        }
    }
}
