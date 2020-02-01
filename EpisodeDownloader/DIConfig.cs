using EpisodeDownloader.Contracts.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace EpisodeDownloader
{
    public class DIConfig : IDiConfig
    {
        public void RegisterTypes(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<EpisodeDownloader>()
                .AddLogging(logging =>
                {
                    logging.AddConfiguration(configuration.GetSection("Logging"));
                    logging.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                });
        }
    }
}
