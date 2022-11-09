using EpisodeDownloader.Contracts.DependencyInjection;
using EpisodeDownloader.Contracts.Downloader;
using EpisodeDownloader.Downloader.Vrt.Models;
using EpisodeDownloader.Downloader.Vrt.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Downloader.Vrt;

public class VrtDiConfig : DiConfigDecorator
{
    public VrtDiConfig(IDiConfig diConfig)
        : base(diConfig)
    {
    }

    public override void RegisterTypes(IServiceCollection services, IConfiguration configuration)
    {
        base.RegisterTypes(services, configuration);
        services.AddTransient<IEpisodeProvider, VrtEpisodeProvider>();
        services.AddSingleton<IVrtTokenService, VrtTokenService>();
        services.Configure<VrtConfiguration>(configuration.GetSection(nameof(VrtEpisodeProvider)));
    }
}
