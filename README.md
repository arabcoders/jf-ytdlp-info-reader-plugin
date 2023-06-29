# YouTube INFO reader plugin

This project based on Ankenyr [jellyfin-youtube-metadata-plugin](/ankenyr/jellyfin-youtube-metadata-plugin), I removed the remote support
and added what we think make sense for episodes numbers, if you follow heavy active channels like we do, you will notice that
some episodes will have problems in sorting or numbering, we fixed some issues that relates to what we need.

## Overview
Plugin for [Jellyfin](https://jellyfin.org/) that retrieves metadata for content from yt-dlp `info.json` files.

### Features
- Reads the `.info.json` files provided by [yt-dlp](https://github.com/yt-dlp/yt-dlp) or similar programs to extract metadata from.
- Supports thumbnails of `png`, `jpg` or `webp` format for both channel and videos.
- Supports the following library types `Movies`, `Music Videos` and `Shows`.
- Supports ExternalID providing quick links to source of metadata.

## Usage

### File Naming Requirements
All media needs to have the ID embedded in the file name within square brackets.
The following are valid examples of a channel and video. We support the following id format
`[(id)]` and `[youtube-(id)]`.

### Channels.
To add metadata from channel, INFO and image should follow the following format. YouTube channels must start with UC or HC.

- `whatever title you want [(youtube-)?(UC|HC)uAXFkgsw1L7xaCfnd5JJOw].info.json`
- `whatever title you want [(youtube-)?(UC|HC)uAXFkgsw1L7xaCfnd5JJOw].(jpg|png|webp)`

### Video files and related metadata.
For Video files it follow the same rules as the channel format.

- `whatever [(youtube-)?dQw4w9WgXcQ].info.json`
- `whatever [(youtube-)?dQw4w9WgXcQ].(jpg|png|webp)`
- `whatever [(youtube-)?dQw4w9WgXcQ].mkv`

## Build and Installing from source

1. Clone or download this repository.
1. Ensure you have .NET Core SDK setup and installed.
1. Build plugin with following command.
    ```
    dotnet publish --configuration Release --output bin
    ```
1. Create folder named `YTINFOReader` in the `plugins` directory inside your Jellyfin data
   directory. You can find your directory by going to Dashboard, and noticing the Paths section.
   Mine is the root folder of the default Metadata directory.
    ```
    # mkdir <Jellyfin Data Directory>/plugins/YTINFOReader/
    ```
1. Place the resulting files from step 3 in the `plugins/YTINFOReader` folder created in step 4.
    ```
    # cp -r bin/*.dll <Jellyfin Data Directory>/plugins/YTINFOReader/`
    ```
1. Be sure that the plugin files are owned by your `jellyfin` user:
    ```
    # chown -R jellyfin:jellyfin /var/lib/jellyfin/plugins/YTINFOReader/
    ```
1. If performed correctly you will see a plugin named YTINFOReader in `Admin -> Dashboard -> Advanced -> Plugins`.