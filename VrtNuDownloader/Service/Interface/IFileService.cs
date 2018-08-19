namespace VrtNuDownloader.Service.Interface
{
    public interface IFileService
    {
        void EnsureFolderExists(string folderName);
        string MakeValidFileName(string name);
        string MakeValidFolderName(string name);
        string ReadFile(string path);
        void WriteFile(string path, string text);
        void MoveFile(string sourceFileName, string destFileName);
    }
}
