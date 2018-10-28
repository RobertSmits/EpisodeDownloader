using System;
using System.Diagnostics;
using VrtNuDownloader.Service.Interface;

namespace VrtNuDownloader.Service
{
    public class FfmpegService : IFfmpegService
    {
        public bool DownloadEpisode(Uri videoUri, string fileName)
        {
            Process p = new Process();
            p.StartInfo.FileName = "ffmpeg";
            p.StartInfo.Arguments = $"-i \"{videoUri.AbsoluteUri}\" -c copy \"{fileName}\"";
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
                //if (line.Contains("time="))
                //{
                //    var progress = line.Substring(line.IndexOf("time=") + 5).Split(" ")[0];
                //    Console.WriteLine("Progress: " + progress);
                //}
            }
            p.WaitForExit();
            return p.ExitCode == 0;
        }
    }
}
