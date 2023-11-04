using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.TV;
using Jellyfin.Plugin.YTINFOReader.Helpers;
using MediaBrowser.Controller.Configuration;
using System.Net.Http;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public class EpisodeProvider : AbstractProvider<EpisodeProvider, Episode, EpisodeInfo>
    {
        public EpisodeProvider(IFileSystem fileSystem, ILogger<EpisodeProvider> logger) : base(fileSystem, logger) { }
        public override string Name => Constants.PLUGIN_NAME;
        internal override MetadataResult<Episode> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToEpisode(jsonObj);
    }
}
