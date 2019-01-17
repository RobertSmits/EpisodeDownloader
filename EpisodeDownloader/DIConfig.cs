
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using EpisodeDownloader.Core.Context;
using EpisodeDownloader.Core.DependencyInjection;

namespace EpisodeDownloader
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
