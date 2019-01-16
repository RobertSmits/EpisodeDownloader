
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using VrtNuDownloader.Core.Context;
using VrtNuDownloader.Core.DependencyInjection;

namespace VrtNuDownloader
{
    public class DIConfig : IDiConfig
    {
        public void RegisterTypes(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<EpisodeDownloader>()
                .AddLogging(logging =>
                {
                    logging.AddConfiguration(Context.Configuration.GetSection("Logging"));
                    logging.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                });
        }
    }
}
