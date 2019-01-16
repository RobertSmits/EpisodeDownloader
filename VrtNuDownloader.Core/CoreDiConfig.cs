using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.DependencyInjection;
using VrtNuDownloader.Core.Downloader;
using VrtNuDownloader.Core.Service.DataBase;
using VrtNuDownloader.Core.Service.Ffmpeg;
using VrtNuDownloader.Core.Service.File;
using VrtNuDownloader.Core.Service.History;

namespace VrtNuDownloader.Core
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
            serviceCollection.Configure<Configuration>(Context.Context.Configuration.GetSection(nameof(VrtNuDownloader)));

        }

        public CoreDiConfig(IDiConfig diConfig)
            : base(diConfig)
        {
        }
    }
}
