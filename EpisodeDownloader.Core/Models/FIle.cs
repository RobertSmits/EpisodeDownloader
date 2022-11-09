namespace EpisodeDownloader.Core.Models;

public class File
{
    public string Name { get; }
    public string Extension { get; }
    public string Path { get; }

    public string GetFullName() => Name + Extension;
    public string GetFullPath() => System.IO.Path.Combine(Path, GetFullName());

    public File SetName(string name) => new File(name, Extension, Path);
    public File SetExtension(string extension) => new File(Name, extension, Path);
    public File SetPath(string path) => new File(Name, Extension, path);

    public File(string name, string extension, string path)
    {
        Name = name;
        Extension = extension;
        Path = path;

        if (!string.IsNullOrEmpty(extension) && extension[0] != '.')
        {
            Extension = "." + Extension;
        }
    }

    public static File FromPath(string path)
    {
        return new File(
            name: System.IO.Path.GetFileNameWithoutExtension(path),
            extension: System.IO.Path.GetExtension(path),
            path: System.IO.Path.GetDirectoryName(path)
        );
    }
}
