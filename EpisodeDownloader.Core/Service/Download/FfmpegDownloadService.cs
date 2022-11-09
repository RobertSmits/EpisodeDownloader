using System;
using System.Diagnostics;
using EpisodeDownloader.Contracts.Exceptions;
using EpisodeDownloader.Core.Models;
using Microsoft.Extensions.Logging;

namespace EpisodeDownloader.Core.Service.Download;

public class FfmpegDownloadService : IDownloadService
{
    private readonly ILogger _logger;

    public FfmpegDownloadService(ILogger<FfmpegDownloadService> logger)
    {
        _logger = logger;
    }

    public void DownloadEpisode(Uri streamUrl, string filePath)
    {
        DownloadEpisode(streamUrl, filePath, TimeSpan.Zero, TimeSpan.Zero);
    }

    public void DownloadEpisode(Uri streamUrl, string filePath, TimeSpan skip, TimeSpan duration)
    {
        var p = new Process();
        p.StartInfo.FileName = "ffmpeg";
        var arguments = new FfmpegArgumentBuilder()
            .AllowOverwrite()
            .HideBanner()
            .Loglevel("panic")
            .StartTimeOffset(skip)
            .Input(streamUrl.AbsoluteUri)
            .Duration(duration)
            .Codec("copy")
            //.CopyTimeStamps()
            .Output(filePath);

        _logger.LogDebug($"Running ffmpeg: ffmpeg {arguments}");

        p.StartInfo.Arguments = arguments.ToString();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardError = true;
        if (!p.Start())
            throw new DownloadException("Error running ffmpeg");

        p.WaitForExit();

        if (p.ExitCode != 0)
            throw new DownloadException("Error running ffmpeg");
    }
}
