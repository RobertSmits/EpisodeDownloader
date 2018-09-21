using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
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
        const string CONFIG_FILE = "config.yaml";
        private readonly IFileService _fileService;
        private Config _config;

        public ConfigService(IFileService fileService)
        {
            _fileService = fileService;
            if (!File.Exists(CONFIG_FILE))
                CreateDefaultConfig();
            else
                LoadConfig();

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
            WriteConfig();
        }

        private void LoadConfig()
        {
            var c = new YamlDotNet.Serialization.Deserializer();
            var fileYaml = _fileService.ReadFile(CONFIG_FILE);
            _config = c.Deserialize<Config>(fileYaml);
        }

        private void WriteConfig()
        {
            var c = new YamlDotNet.Serialization.Serializer();
            var fileYaml = c.Serialize(_config);
            _fileService.WriteFile(CONFIG_FILE, fileYaml);
        }


        public string Email => _config.Email;

        public string Password => _config.Password;
        public string Cookie => _config.Cookie;

        public string DownloadPath => _config.DownloadPath;

        public string SavePath => _config.SavePath;

        public bool SaveShowsInFolders => _config.SaveShowsInFolders;

        public bool SaveSeasonsInFolders => _config.SaveSeasonsInFolders;

        public IEnumerable<string> WatchUrls => _config.WatchUrls;
    }
}
