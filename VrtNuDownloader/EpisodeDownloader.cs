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
            var defaultDownloader = DependencyInjectionConfig.Container.GetService<DefaultDownloader>();

            foreach (var showUrl in ShowUrls)
            {
                var handler = DependencyInjectionConfig.Container
                    .GetServices<IDownloader>()
                    .FirstOrDefault(x => x.CanHandleUrl(showUrl))
                    ?? defaultDownloader;

                handler.Handle(showUrl);
            }
        }
    }
}
