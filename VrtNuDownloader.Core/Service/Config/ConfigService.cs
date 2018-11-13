using System.Collections.Generic;
using VrtNuDownloader.Core.Service.File;

namespace VrtNuDownloader.Core.Service.Config
{
    public class ConfigService : IConfigService
    {
        public class Config : IConfigService
        {
            public string VrtCookie { get; set; }
            
            public string VierLastAuthUser { get; set; }
            public string VierRefreshToken { get; set; }

            public string DownloadPath { get; set; }
            public string SavePath { get; set; }
            public bool SaveShowsInFolders { get; set; }
            public bool SaveSeasonsInFolders { get; set; }
            public IEnumerable<string> WatchUrls { get; set; }
        }


        const string DOWNLOAD_DIR = "Downloads";
        const string CONFIG_FILE = "config.yml";
        private readonly IFileService _fileService;
        private Config _config;

        public ConfigService(IFileService fileService)
        {
            _fileService = fileService;
            if (!System.IO.File.Exists(CONFIG_FILE))
                CreateDefaultConfig();
            else
                _config = _fileService.ReadYamlFile<Config>(CONFIG_FILE);

            _fileService.EnsureFolderExists(_config.DownloadPath);
            _fileService.EnsureFolderExists(_config.SavePath);
        }

        private void CreateDefaultConfig()
        {
            _config = new Config
            {
                DownloadPath = DOWNLOAD_DIR,
                SavePath = DOWNLOAD_DIR,
                SaveShowsInFolders = true,
                SaveSeasonsInFolders = true,
                WatchUrls = new string[0]
            };
            _fileService.WriteYamlFile(_config, CONFIG_FILE);
        }

        public string VrtCookie
        {
            get => _config.VrtCookie;
            set
            {
                _config.VrtCookie = value;
                _fileService.WriteYamlFile(_config, CONFIG_FILE);
            }
        }
        
        public string VierLastAuthUser =>_config.VierLastAuthUser;
        public string VierRefreshToken => _config.VierRefreshToken;
        
        public string DownloadPath => _config.DownloadPath;
        public string SavePath => _config.SavePath;
        public bool SaveShowsInFolders => _config.SaveShowsInFolders;
        public bool SaveSeasonsInFolders => _config.SaveSeasonsInFolders;
        public IEnumerable<string> WatchUrls => _config.WatchUrls;
    }
}
