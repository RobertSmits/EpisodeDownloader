# EpisodeDownloader

A downloader for [VRT Nu](https://www.vrt.be/vrtnu/) and [Vier](https://www.vier.be/)

## Requirements

This project uses the .NET Core 3.1 runtime.
To build this project you need to have the .NET Core  SDK installed on your system.
If you only want to run the compiled binary the .NET Core 3.1 Runtime will do.

[download link](https://github.com/dotnet/core/tree/master/release-notes/3.1)

## Building

Building the project can be done from Visual Studio or from the comand line

``` bash
$ dotnet build -c Release
```

## Running

After building the project you can run the program by navigating to the outpt folder and starting the dotnet executable

``` bash
$ cd bin/Release/netcoreapp3.1
$ dotnet ./EpisodeDownloader
```

## Configuration

Program configuration is done using the `config.yml` file.
Using your preffered editor you can change the configuration according to the [YAML](http://yaml.org/spec/1.1/) spesificaton.

### Basic Configuration

The `EpisodeDownloader` section of the configuration file contains the core configuration

``` yaml
EpisodeDownloader:
    DownloadPath: /tmp          # Path where new episodes wil be downloaded to
    SavePath: Downloads         # Completed episodes wil be moved here
    Overwrite: false            # Overwrite file if destiantion alreadey exists
    SaveShowsInFolders: true    # Create separate sub folders per show
    SaveSeasonsInFolders: true  # Create separate sub folders per season
    DownloadExtension: mkv      # The output file extension
    WatchUrls:                  # Urls to watch
        - https://www.vrt.be/vrtnu/a-z/w817.relevant/
        - https://www.vrt.be/vrtnu/a-z/thuis.relevant/
```

# Plugins

## VrtDownloader

Plugin providing downloader for [VRT Nu](https://www.vrt.be/vrtnu/)

### Configuration

This plugin adds folowing section to `config.yml`

``` yaml
VrtDownloader:
    Email:      # Your VrtNu email
    Password:   # Your VrtNu password
```

### Authentication

This plugin uses email address and password for authentication, these need to be added in the config file

### Show URLs

To find the show url to add, search your show on VRT Nu and copy the url.
This url looks like this:
```
https://www.vrt.be/vrtnu/a-z/{{Show}}/{{Season}}/{{Episode}}/
```
This is an episode specific url and will stop working is the episode gets removed.

To keep our downloader working we need to moddify our url to reflect a show url.
The show url is acquired by stripping {{Season}}/ and {{Episode}}/ from the url and adding `.relevant` after {{Show}}.
The url should now look like this:
```
https://www.vrt.be/vrtnu/a-z/{{Show}}.relevant/
```

__Note the tailing `/`__

#### Example:

```
Episode url:
https://www.vrt.be/vrtnu/a-z/thuis/24/thuis-s24a4464/

Show url:
https://www.vrt.be/vrtnu/a-z/thuis.relevant/
```

### Config Sample

``` yaml
EpisodeDownloader:
    WatchUrls:
        - https://www.vrt.be/vrtnu/a-z/w817.relevant/
        - https://www.vrt.be/vrtnu/a-z/thuis.relevant/
VrtDownloader:
    Email: foo@bar.com
    Password:  FooBar
```

## VierDownloader

Plugin providing downloader for [Vier](https://www.vier.be/)

### Configuration

This plugin adds folowing section to `config.yml`

``` yaml
VierDownloader:
    Email:      # Your Vier email
    Password:   # Your Vier password

```

### Authentication

This plugin uses email address and password for authentication, these need to be added in the config file

### Show URLs

TODO

#### Example:

```
https://www.vier.be/video/callboys/callboys-seizoen-1/
```

### Config Sample

``` yaml
EpisodeDownloader:
    WatchUrls:
        - https://www.vier.be/video/de-dag/de-dag/de-dag-aflevering-1/
        - https://www.vier.be/video/callboys/callboys-seizoen-1/
VierDownloader:
    Email: foo@bar.com
    Password:  FooBar
```
