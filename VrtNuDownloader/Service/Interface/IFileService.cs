namespace VrtNuDownloader.Service.Interface
{
    public interface IFileService
    {
        string DownloadDir { get; }
        string WatchFilePath { get; }
        string CheckFilePath { get; }
        void EnsureFolderExists(string folderName);
        string MakeValidFileName(string name);
        string MakeValidFolderName(string name);
    }
}
