using System;
using Unity;
using VrtNuDownloader.Service;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container)
        {            container.RegisterType<ILogService, LogService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IVrtNuService, VrtNuService>();
            container.RegisterType<IFfmpegService, FfmpegService>();
            container.RegisterType<IHistoryService, HistoryService>();
            container.RegisterType<IDatabaseService, DatabaseService>();
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
