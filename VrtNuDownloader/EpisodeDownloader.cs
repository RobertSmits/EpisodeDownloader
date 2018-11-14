using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.Interfaces;

namespace VrtNuDownloader
{
    public class EpisodeDownloader
    {
        public void Run(IEnumerable<Uri> ShowUrls)
        {
            foreach (var showUrl in ShowUrls)
            {
                DependencyInjectionConfig.Container
                    .GetServices<IDownloader>()
                    .First(x => x.CanHandleUrl(showUrl))
                    .Handle(showUrl);
            }
        }
    }
}
