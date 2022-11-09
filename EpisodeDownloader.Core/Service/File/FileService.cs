using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EpisodeDownloader.Core.Service.File;

public class FileService : IFileService
{
    public bool CheckFileExists(string fileName)
    {
        return System.IO.File.Exists(fileName);
    }

    public void EnsureFolderExists(string folderName)
    {
        if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
    }

    public string MakeValidFileName(string name)
    {
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
        return Regex.Replace(name.Replace("\"", "'"), invalidRegStr, "-");
    }

    public string MakeValidFolderName(string name)
    {
        string invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars().Concat(new[] { '\\', '/' }).ToArray()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
        return Regex.Replace(name, invalidRegStr, "-");
    }

    public string ReadFile(string path)
    {
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fileStream);
        return reader.ReadToEnd();
    }

    public void WriteFile(string path, string text)
    {
        using var writer = new StreamWriter(path, false);
        writer.Write(text);
    }

    public void MoveFile(string sourceFileName, string destFileName, bool overwrite)
    {
        string extension = Path.GetExtension(destFileName);
        string pathName = Path.GetDirectoryName(destFileName);
        string fileNameOnly = Path.Combine(pathName, Path.GetFileNameWithoutExtension(destFileName));

        if (!overwrite)
        {
            int i = 0;
            // If the file exists, keep trying until it doesn't
            while (System.IO.File.Exists(destFileName))
            {
                i += 1;
                destFileName = string.Format("{0} ({1}){2}", fileNameOnly, i, extension);
            }
        }

        System.IO.File.Move(sourceFileName, destFileName, overwrite);
    }
}
