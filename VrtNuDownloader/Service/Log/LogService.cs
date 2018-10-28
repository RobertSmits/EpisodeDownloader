using System;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
{
    public class LogService : ILogService
    {
        public void WriteLog(MessageType type, string message)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
            Console.WriteLine($"{time}|{type.ToString().PadRight(6)}|   |{message}");
        }
    }
}
