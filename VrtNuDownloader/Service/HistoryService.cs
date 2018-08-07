using System.IO;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
{
    public class HistoryService : IHistoryService
    {
        private readonly IFileService _fileService;
        
        public HistoryService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void AddDownloaded(string slug)
        {
            File.AppendAllLines(_fileService.CheckFilePath, new[] { slug });
        }

        public bool CheckIfDownloaded(string slug)
        {
            using (StreamReader f = new StreamReader(_fileService.CheckFilePath))
            {
                string line;
                while ((line = f.ReadLine()) != null)
                {
                    if (line == slug) return true;
                }
            }
            return false;
        }
    }
}
