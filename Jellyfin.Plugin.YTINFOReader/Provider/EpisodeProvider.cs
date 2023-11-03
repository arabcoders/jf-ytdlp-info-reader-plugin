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
        public EpisodeProvider(
            IFileSystem fileSystem,
            IHttpClientFactory httpClientFactory,
            ILogger<EpisodeProvider> logger,
            IServerConfigurationManager config,
            System.IO.Abstractions.IFileSystem afs) : base(fileSystem, httpClientFactory, logger, config, afs)
        {
        }

        public override string Name => Constants.PLUGIN_NAME;

        internal override MetadataResult<Episode> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToEpisode(jsonObj);

    }
}
