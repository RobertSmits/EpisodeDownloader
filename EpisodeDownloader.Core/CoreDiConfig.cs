using EpisodeDownloader.Contracts.DependencyInjection;
using EpisodeDownloader.Core.Downloader;
using EpisodeDownloader.Core.Service.Download;
using EpisodeDownloader.Core.Service.File;
using EpisodeDownloader.Core.Service.History;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Core;

public class CoreDiConfig : DiConfigDecorator
{
    public CoreDiConfig(IDiConfig diConfig)
        : base(diConfig)
    {
    }

    public override void RegisterTypes(IServiceCollection services, IConfiguration configuration)
    {
        base.RegisterTypes(services, configuration);
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IHistoryService, HistoryService>();
        services.AddTransient<IDownloadService, FfmpegDownloadService>();
        services.AddTransient<DefaultDownloader>();
        services.Configure<Configuration>(configuration.GetSection(nameof(EpisodeDownloader)));
    }
}
