using System;
using System.Diagnostics;
using System.IO;
using VrtNuDownloader.Core.Service.File;

namespace VrtNuDownloader.Core.Service.Ffmpeg
{
    public class FfmpegService : IFfmpegService
    {
        private readonly IFileService _fileService;
        public FfmpegService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public bool DownloadAndMoveEpisode(Uri videoUri, string fileName, string downloadPath, string savePath)
        {
            var downloadFilePath = Path.Combine(downloadPath, fileName);
            var result = DownloadEpisode( videoUri, downloadFilePath);
            if (result == false) return false;


            savePath = Path.Combine(savePath, fileName);
            _fileService.MoveFile(downloadFilePath, savePath);
            return true;
        }

        public bool DownloadEpisode(Uri videoUri, string filePath)
        {
            Process p = new Process();
            p.StartInfo.FileName = "ffmpeg";
            p.StartInfo.Arguments = $"-i \"{videoUri.AbsoluteUri}\" -c copy \"{filePath}\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            if (!p.Start()) return false;
            string line;
            while ((line = p.StandardError.ReadLine()) != null)
            {
                if (line.Contains("Duration: "))
                {
                    var time = line.Split(",")[0].Replace("  Duration: ", "");
                    Console.WriteLine("Lengte aflevering: " + time);
                }
            }
            p.WaitForExit();
            return p.ExitCode == 0;
        }
    }
}
