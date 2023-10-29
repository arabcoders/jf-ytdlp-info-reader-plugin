using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.TV;
using Jellyfin.Plugin.YTINFOReader.Helpers;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public class LocalEpisodeProvider : AbstractLocalProvider<LocalEpisodeProvider, Episode>
    {
        public override string Name => Constants.PLUGIN_NAME;

        public LocalEpisodeProvider(IFileSystem fileSystem, ILogger<LocalEpisodeProvider> logger) : base(fileSystem, logger) { }

        internal override MetadataResult<Episode> GetMetadataImpl(YTDLData jsonObj)
        {
            return Utils.YTDLJsonToEpisode(jsonObj);
        }
    }
}
