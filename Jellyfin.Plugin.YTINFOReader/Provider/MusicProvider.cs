using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using Jellyfin.Plugin.YTINFOReader.Helpers;
using MediaBrowser.Controller.Configuration;
using System.Net.Http;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public class MusicProvider : AbstractProvider<MusicProvider, MusicVideo, MusicVideoInfo>
    {
        public MusicProvider(
            IFileSystem fileSystem,
            IHttpClientFactory httpClientFactory,
            ILogger<MusicProvider> logger,
            IServerConfigurationManager config,
            System.IO.Abstractions.IFileSystem afs) : base(fileSystem, httpClientFactory, logger, config, afs)
        {
        }

        public override string Name => Constants.PLUGIN_NAME;

        internal override MetadataResult<MusicVideo> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToMusicVideo(jsonObj);

    }
}
