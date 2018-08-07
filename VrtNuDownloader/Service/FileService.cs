using System.IO;
using System.Text.RegularExpressions;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
{
    public class FileService : IFileService
    {
        const string DOWNLOAD_DIR = "Downloads";
        const string CONFIG_DIR = "Files";
        const string WATCH_FILE = "watchlist.txt";
        const string CHECK_FILE = "done.txt";

        public FileService()
        {
            EnsureFolderExists(DOWNLOAD_DIR);
            EnsureFolderExists(CONFIG_DIR);
            WatchFilePath = Path.Combine(CONFIG_DIR, WATCH_FILE);
            CheckFilePath = Path.Combine(CONFIG_DIR, CHECK_FILE);
            if (!File.Exists(WatchFilePath)) File.Create(WatchFilePath).Dispose();
            if (!File.Exists(CheckFilePath)) File.Create(CheckFilePath).Dispose();
        }


        public string DownloadDir { get; private set; } = DOWNLOAD_DIR;
        public string WatchFilePath { get; private set; }
        public string CheckFilePath { get; private set; }


        public void EnsureFolderExists(string folderName)
        {
            if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
        }

        public string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(name, invalidRegStr, "_");
        }

        public string MakeValidFolderName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(name, invalidRegStr, "_");
        }

    }
}
