using System;
using System.Collections.Generic;

namespace EpisodeDownloader.Core.Models;

public class FfmpegArgumentBuilder
{
    private readonly IList<string> _arguments;

    public FfmpegArgumentBuilder()
    {
        _arguments = new List<string>();
    }

    public FfmpegArgumentBuilder AllowOverwrite()
    {
        _arguments.Add("-y");
        return this;
    }

    public FfmpegArgumentBuilder HideBanner()
    {
        _arguments.Add("-hide_banner");
        return this;
    }

    public FfmpegArgumentBuilder Loglevel(string level)
    {
        _arguments.Add($"-loglevel {level}");
        return this;
    }

    public FfmpegArgumentBuilder Input(string url)
    {
        _arguments.Add($"-i \"{url}\"");
        return this;
    }

    public FfmpegArgumentBuilder Output(string path)
    {
        _arguments.Add($"\"{path}\"");
        return this;
    }

    public FfmpegArgumentBuilder StartTimeOffset(TimeSpan offset)
    {
        var twoSeconds = new TimeSpan(0, 0, 2);
        if (offset > twoSeconds)
        {
            _arguments.Add($"-ss {offset.Add(twoSeconds.Negate()):hh\\:mm\\:ss}");
        }
        return this;
    }

    public FfmpegArgumentBuilder Duration(TimeSpan duration)
    {
        if (duration.TotalSeconds > 0)
        {
            _arguments.Add($"-t {duration.TotalSeconds + 4}");
        }
        return this;
    }

    public FfmpegArgumentBuilder StopTime(TimeSpan stopTime)
    {
        _arguments.Add($"-to {stopTime:hh\\:mm\\:ss}");
        return this;
    }

    public FfmpegArgumentBuilder Codec(string codec)
    {
        _arguments.Add($"-c {codec}");
        return this;
    }

    public FfmpegArgumentBuilder CopyTimeStamps()
    {
        _arguments.Add("-copyts");
        return this;
    }

    public override string ToString()
    {
        return string.Join(" ", _arguments);
    }
}
