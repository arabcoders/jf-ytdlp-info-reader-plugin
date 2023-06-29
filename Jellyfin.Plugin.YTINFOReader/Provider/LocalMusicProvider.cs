using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using Jellyfin.Plugin.YTINFOReader.Helpers;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public class LocalMusicProvider : AbstractLocalProvider<LocalMusicProvider, MusicVideo>
    {
        public override string Name => Constants.PLUGIN_NAME;
        public LocalMusicProvider(IFileSystem fileSystem, ILogger<LocalMusicProvider> logger) : base(fileSystem, logger) { }
        internal override MetadataResult<MusicVideo> GetMetadataImpl(YTDLData jsonObj)
        {
            return Utils.YTDLJsonToMusicVideo(jsonObj);
        }
    }
}
