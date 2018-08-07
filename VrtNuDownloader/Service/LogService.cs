using System;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
{
    public class LogService : ILogService
    {
        public void WriteLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
