using System;
using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Service;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader
{
    public class DependencyInjectionConfig
    {
        public static void RegisterTypes(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILogService, LogService>();
            serviceCollection.AddSingleton<IFileService, FileService>();
            serviceCollection.AddSingleton<IVrtNuService, VrtNuService>();
            serviceCollection.AddSingleton<IFfmpegService, FfmpegService>();
            serviceCollection.AddSingleton<IConfigService, ConfigService>();
            serviceCollection.AddSingleton<IHistoryService, HistoryService>();
            serviceCollection.AddSingleton<IVrtTokenService, VrtTokenService>();
            serviceCollection.AddSingleton<IDatabaseService, DatabaseService>();
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
