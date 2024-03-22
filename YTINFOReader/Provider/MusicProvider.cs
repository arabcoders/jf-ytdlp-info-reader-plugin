using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using YTINFOReader.Helpers;

namespace YTINFOReader;

public class MusicProvider : AbstractProvider<MusicProvider, MusicVideo, MusicVideoInfo>
{
    public MusicProvider(IFileSystem fileSystem, ILogger<MusicProvider> logger) : base(fileSystem, logger) { }

    internal override MetadataResult<MusicVideo> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToMusicVideo(jsonObj);
}
