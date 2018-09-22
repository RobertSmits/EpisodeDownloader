using System;
using Unity;
using VrtNuDownloader.Service;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container)
        {

            container.RegisterSingleton<ILogService, LogService>();
            container.RegisterSingleton<IFileService, FileService>();
            container.RegisterSingleton<IVrtNuService, VrtNuService>();
            container.RegisterSingleton<IFfmpegService, FfmpegService>();
            container.RegisterSingleton<IConfigService, ConfigService>();
            container.RegisterSingleton<IHistoryService, HistoryService>();
            container.RegisterSingleton<IVrtTokenService, VrtTokenService>();
            container.RegisterSingleton<IDatabaseService, DatabaseService>();
        }

        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        public static IUnityContainer Container => container.Value;
    }
}
