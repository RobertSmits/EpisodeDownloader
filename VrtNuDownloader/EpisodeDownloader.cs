using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.Interfaces;
using VrtNuDownloader.Core.Service.Logging;

namespace VrtNuDownloader
{
    public class EpisodeDownloader
    {
        public void Run(IEnumerable<Uri> ShowUrls)
        {
            foreach (var showUrl in ShowUrls)
            {
                var handler = DependencyInjectionConfig.Container
                    .GetServices<IDownloader>()
                    .FirstOrDefault(x => x.CanHandleUrl(showUrl));

                if (handler != null)
                {
                    handler.Handle(showUrl);
                    continue;
                }

                DependencyInjectionConfig.Container.GetService<ILoggingService>().WriteLog(MessageType.Error, $"No handler found for url: {showUrl}");
            }
        }
    }
}
