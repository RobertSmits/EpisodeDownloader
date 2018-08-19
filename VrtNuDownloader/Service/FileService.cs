using System.IO;
using System.Text.RegularExpressions;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
{
    public class FileService : IFileService
    {
        public void EnsureFolderExists(string folderName)
        {
            if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
        }

        public string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(name, invalidRegStr, "-");
        }

        public string MakeValidFolderName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(name, invalidRegStr, "-");
        }

        public string ReadFile(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            {
                return reader.ReadToEnd();
            }
        }

        public void WriteFile(string path, string text)
        {
            using (var writer = new StreamWriter(path, false))
            {
                writer.Write(text);
            }
        }

        public void MoveFile(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }
    }
}
