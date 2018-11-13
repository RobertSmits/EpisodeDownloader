using System.Collections.Generic;
using System.IO;
using System.Linq;
using VrtNuDownloader.Core.Service.File;

namespace VrtNuDownloader.Core.Models
{
    public class SavePathBuilder
    {
        private EpisodeInfo _episodeInfo;
        private string _basePath;
        private IList<string> _pathSections;
        private readonly IFileService _fileService;

        public SavePathBuilder(IFileService fileService)
        {
            _fileService = fileService;
            _pathSections = new List<string>();
        }

        public SavePathBuilder ForEpisode(EpisodeInfo episodeInfo)
        {
            _episodeInfo = episodeInfo;
            return this;
        }

        public SavePathBuilder SetBasePath(string basePath)
        {
            _basePath = basePath;
            return this;
        }

        public SavePathBuilder AddShowFolder()
        {
            var folderName = _fileService.MakeValidFileName(_episodeInfo.ShowName);
            _pathSections.Add(folderName);
            return this;
        }

        public SavePathBuilder AddSeasonFolder()
        {
            var season = int.TryParse(_episodeInfo.Season, out int seasonNr)
                ? $"S{seasonNr:00}"
                : _episodeInfo.Season;
            var folderName = _fileService.MakeValidFileName(season);

            _pathSections.Add(folderName);
            return this;
        }

        public SavePathBuilder AddPathSection(string path)
        {
            _pathSections.Add(path);
            return this;
        }

        public string Build()
        {
            return _pathSections.Aggregate(_basePath, (path, section) => Path.Combine(path, section));
        }
    }
}
