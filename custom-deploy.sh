#!/usr/bin/env bash

set -eou pipefail

if [ -d "/home/coders/apps/jf-ytdlp-info-reader-plugin/bin/" ]; then
    rm -r /home/coders/apps/jf-ytdlp-info-reader-plugin/bin/
fi

if [ -d "/home/coders/apps/jf-ytdlp-info-reader-plugin/Jellyfin.Plugin.YTINFOReader/bin/" ]; then
    rm -r /home/coders/apps/jf-ytdlp-info-reader-plugin/Jellyfin.Plugin.YTINFOReader/bin/
fi

cd /home/coders/apps/jf-ytdlp-info-reader-plugin

dotnet test

dotnet publish Jellyfin.Plugin.YTINFOReader --configuration Release --output /home/coders/apps/jf-ytdlp-info-reader-plugin/bin

rm -v /home/coders/.docker/data/jellyfin/data/plugins/YTINFOReader/*

cp -v /home/coders/apps/jf-ytdlp-info-reader-plugin/bin/*.dll /home/coders/.docker/data/jellyfin/data/plugins/YTINFOReader/
