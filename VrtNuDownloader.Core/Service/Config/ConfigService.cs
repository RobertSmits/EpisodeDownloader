using System.Collections.Generic;
using System.IO;
using VrtNuDownloader.Core.Service.File;

namespace VrtNuDownloader.Core.Service.Config
{
    public class ConfigService : IConfigService
    {
        public class Config : IConfigService
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Cookie { get; set; }

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
                Email = null,
                Password = null,
                Cookie = null,
                DownloadPath = DOWNLOAD_DIR,
                SavePath = DOWNLOAD_DIR,
                SaveShowsInFolders = true,
                SaveSeasonsInFolders = true,
                WatchUrls = new string[0]
            };
            _fileService.WriteYamlFile(_config, CONFIG_FILE);
        }


        public string Email => _config.Email;
        public string Password => _config.Password;
        public string Cookie
        {
            get => _config.Cookie;
            set
            {
                _config.Cookie = value;
                _fileService.WriteYamlFile(_config, CONFIG_FILE);
            }
        }

        public string DownloadPath => _config.DownloadPath;
        public string SavePath => _config.SavePath;
        public bool SaveShowsInFolders => _config.SaveShowsInFolders;
        public bool SaveSeasonsInFolders => _config.SaveSeasonsInFolders;
        public IEnumerable<string> WatchUrls => _config.WatchUrls;
    }
}
