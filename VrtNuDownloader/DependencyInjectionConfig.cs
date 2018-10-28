using System;
using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.Interfaces;
using VrtNuDownloader.Core.Service.Config;
using VrtNuDownloader.Core.Service.DataBase;
using VrtNuDownloader.Core.Service.Ffmpeg;
using VrtNuDownloader.Core.Service.File;
using VrtNuDownloader.Core.Service.History;
using VrtNuDownloader.Core.Service.Logging;
using VrtNuDownloader.Downloader.Vrt;
using VrtNuDownloader.Downloader.Vrt.Service;

namespace VrtNuDownloader
{
    public class DependencyInjectionConfig
    {
        public static void RegisterTypes(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILoggingService, LoggingService>();
            serviceCollection.AddSingleton<IFileService, FileService>();
            serviceCollection.AddSingleton<IVrtNuService, VrtNuService>();
            serviceCollection.AddSingleton<IFfmpegService, FfmpegService>();
            serviceCollection.AddSingleton<IConfigService, ConfigService>();
            serviceCollection.AddSingleton<IHistoryService, HistoryService>();
            serviceCollection.AddSingleton<IVrtTokenService, VrtTokenService>();
            serviceCollection.AddSingleton<IDatabaseService, DatabaseService>();
            serviceCollection.AddSingleton<IDownloader, VrtDownloader>();
            serviceCollection.AddTransient<EpisodeDownloader>();
        }

        private static Lazy<ServiceProvider> container =
          new Lazy<ServiceProvider>(() =>
          {
              var serviceCollection = new ServiceCollection();
              RegisterTypes(serviceCollection);
              return serviceCollection.BuildServiceProvider();
          });

        public static ServiceProvider Container => container.Value;
    }
}
