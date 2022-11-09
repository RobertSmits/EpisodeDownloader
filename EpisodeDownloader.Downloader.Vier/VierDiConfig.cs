using EpisodeDownloader.Contracts.DependencyInjection;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Downloader.Vier.Models;
using EpisodeDownloader.Downloader.Vier.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Downloader.Vier;

public class VierDiConfig : DiConfigDecorator
{
    public VierDiConfig(IDiConfig diConfig)
        : base(diConfig)
    {
    }

    public override void RegisterTypes(IServiceCollection services, IConfiguration configuration)
    {
        base.RegisterTypes(services, configuration);
        services.AddTransient<IEpisodeProvider, VierEpisodeProvider>();
        services.AddSingleton<IVierAuthService, VierAuthService>();
        services.Configure<VierConfiguration>(configuration.GetSection(nameof(VierEpisodeProvider)));
    }
}
