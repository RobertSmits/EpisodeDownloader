using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VrtNuDownloader.Core.DependencyInjection;
using VrtNuDownloader.Core.Service.Config;
using VrtNuDownloader.Core.Service.DataBase;
using VrtNuDownloader.Core.Service.Ffmpeg;
using VrtNuDownloader.Core.Service.File;
using VrtNuDownloader.Core.Service.History;
using VrtNuDownloader.Core.Service.Logging;

namespace VrtNuDownloader
{
    public class DependencyInjectionConfig : IDiConfig
    {
        public void RegisterTypes(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILoggingService, LoggingService>();
            serviceCollection.AddSingleton<IFileService, FileService>();
            serviceCollection.AddSingleton<IConfigService, ConfigService>();
            serviceCollection.AddSingleton<IHistoryService, HistoryService>();
            serviceCollection.AddSingleton<IDatabaseService, DatabaseService>();
            serviceCollection.AddTransient<IFfmpegService, FfmpegService>();
            serviceCollection.AddTransient<EpisodeDownloader>();
        }

        private static Lazy<ServiceProvider> container = new Lazy<ServiceProvider>(() =>
        {
            var serviceCollection = new ServiceCollection();
            var config = LoadAllDiConfigs();
            config.RegisterTypes(serviceCollection);
            return serviceCollection.BuildServiceProvider();
        });

        private static IDiConfig LoadAllDiConfigs()
        {
            var dllFileNames = Directory.GetFiles("./", "VrtNuDownloader.Downloader.*.dll");
            var assemblies = dllFileNames.Select(Assembly.LoadFrom)
              .ToList();
            var decoratorTypes = assemblies.SelectMany(a => a.GetTypes())
              .Where(t => !t.IsInterface && !t.IsAbstract)
              .Where(t => t.IsSubclassOf(typeof(DiConfigDecorator)));

            IDiConfig config = new DependencyInjectionConfig();
            config = decoratorTypes.Aggregate(config, (c, t) => (DiConfigDecorator)Activator.CreateInstance(t, new object[] { c }));

            return config;
        }

        public static ServiceProvider Container => container.Value;
    }
}
