namespace VrtNuDownloader.Service.Interface
{
    public interface IHistoryService
    {
        void AddDownloaded(string slug);
        bool CheckIfDownloaded(string slug);
    }
}
