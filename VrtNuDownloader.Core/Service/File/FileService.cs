using System.IO;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace VrtNuDownloader.Core.Service.File
{
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
            System.IO.File.Move(sourceFileName, destFileName);
        }


        public T ReadYamlFile<T>(string fileName)
        {
            var fileYaml = ReadFile(fileName);
            return new Deserializer().Deserialize<T>(fileYaml);
        }

        public void WriteYamlFile<T>(T data, string fileName)
        {
            var serialiser = new SerializerBuilder().EmitDefaults().Build();
            var fileYaml = serialiser.Serialize(data);
            WriteFile(fileName, fileYaml);
        }
    }
}
