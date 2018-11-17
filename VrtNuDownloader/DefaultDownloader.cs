using System;
using VrtNuDownloader.Core.Interfaces;
using VrtNuDownloader.Core.Service.Logging;

namespace VrtNuDownloader
{
    public class DefaultDownloader : IDownloader
    {
        private readonly ILoggingService _loggingService;

        public DefaultDownloader(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public bool CanHandleUrl(Uri episodeUrl)
        {
            return false;
        }

        public void Handle(Uri episodeUrl)
        {
            _loggingService.WriteLog(MessageType.Error, $"No handler found for url: {episodeUrl}");
        }
    }
}
