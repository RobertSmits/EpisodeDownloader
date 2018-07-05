using System.IO;
using System.Text.RegularExpressions;

namespace VrtNuDownloader
{
    public class FileHelper
    {
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
