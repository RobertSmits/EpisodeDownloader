using System;

namespace VrtNuDownloader.Core.Service.Logging
{
    public class LoggingService : ILoggingService
    {
        public void WriteLog(MessageType type, string message)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
            Console.WriteLine($"{time}|{type.ToString().PadRight(6)}|   |{message}");
        }
    }
}
