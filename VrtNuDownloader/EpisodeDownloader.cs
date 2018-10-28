using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.Interfaces;

namespace VrtNuDownloader
{
    public class EpisodeDownloader
    {
        public void Run(IEnumerable<Uri> ShowUris)
        {
            foreach (var showUri in ShowUris)
            {
                DependencyInjectionConfig.Container
                    .GetServices<IDownloader>()
                    .First(x => x.CanHandleUrl(showUri))
                    .Handle(showUri);
            }
        }
    }
}
