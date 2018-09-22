# VrtNuDownloader

A downloader for [VRT Nu](https://www.vrt.be/vrtnu/)

## Building

This project uses the .NET Core 2.0 runtime.
To build this project you need to have the .NET Core 2.0 SDK installed on your system. [download link](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.9-download.md)


Building the project can be done from Visual Studio or from the comand line

```
> dotnet publish -c Release
> cd bin\Release\netcoreapp2.0
> dotnet .\VrtNuDownloader
```

## Configuration

When the program runs the first time it wil create a configuration file `config.yml` and a SQLite 3 database `config.db`.
Using your preffered editor you can change the configuration according to the [YAML](http://yaml.org/spec/1.1/) spesificaton.

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

### Cookies

This program uses a cookie for authentication, this needs to be added in the config file.

To get this cookie go to [VRT Nu](https://www.vrt.be/vrtnu/) and log in.

Using your browser developer tools find the sites cookies and search for a cookie named `vrtlogin-rt`.
Copy this cookies contents and place it in the config file. Now delete your browser cookies on VRT Nu.

After the first download your config's cookie will be cleared and you wil see a new file named `.cookies.yml`.
This file contains all cookies needed and their expiration times.
__Dont delete this file!__ 

### Config Example

```
Cookie: a verry long secret string
DownloadPath: Downloads
SavePath: Downloads
SaveShowsInFolders: true
SaveSeasonsInFolders: true
WatchUrls:
- https://www.vrt.be/vrtnu/a-z/w817.relevant/
- https://www.vrt.be/vrtnu/a-z/thuis.relevant/

```

## Run

You are now ready to download your shows.
Run the program again and it will start downloading.

## Other Options

The config file has a few more options explained here

| Option | Type | Usage |
| --- | --- | --- |
| Email | string | Not Used |
| Password | string | Not Used |
| Cookie | string | Vrt Nu refresh token, used for login on initial run |
| DownloadPath | string | Path where files get downloaded |
| SavePath | string | Path where files get placed after download |
| SaveShowsInFolders | boolean | In the DownloadPath create separate folders for each show |
| SaveSeasonsInFolders | boolean | reate separate folders for each season |
| WatchUrls | array | Shows to watch |
