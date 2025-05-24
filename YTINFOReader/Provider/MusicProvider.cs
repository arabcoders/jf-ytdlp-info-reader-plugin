﻿using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using YTINFOReader.Helpers;

namespace YTINFOReader.Provider;

public class MusicProvider : AbstractProvider<MusicProvider, MusicVideo, MusicVideoInfo>
{
    public MusicProvider(IFileSystem fileSystem, ILogger<MusicProvider> logger) : base(fileSystem, logger) { }
    public override string Name => $"{Constants.PLUGIN_NAME}: Music Provider";

    internal override MetadataResult<MusicVideo> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToMusicVideo(jsonObj);
}
