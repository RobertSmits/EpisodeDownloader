using EpisodeDownloader.Core.DependencyInjection;
using EpisodeDownloader.Core.Downloader;
using EpisodeDownloader.Core.Service.DataBase;
using EpisodeDownloader.Core.Service.Ffmpeg;
using EpisodeDownloader.Core.Service.File;
using EpisodeDownloader.Core.Service.History;
using Microsoft.Extensions.DependencyInjection;

namespace EpisodeDownloader.Core
{
    public class CoreDiConfig : DiConfigDecorator
    {
        public override void RegisterTypes(IServiceCollection serviceCollection)
        {
            base.RegisterTypes(serviceCollection);
            serviceCollection.AddSingleton<IFileService, FileService>();
            serviceCollection.AddSingleton<IHistoryService, HistoryService>();
            serviceCollection.AddSingleton<IDatabaseService, DatabaseService>();
            serviceCollection.AddTransient<IFfmpegService, FfmpegService>();
            serviceCollection.AddTransient<DefaultDownloader>();
            serviceCollection.Configure<Configuration>(Context.Context.Configuration.GetSection(nameof(EpisodeDownloader)));

        }

        public CoreDiConfig(IDiConfig diConfig)
            : base(diConfig)
        {
        }
    }
}
