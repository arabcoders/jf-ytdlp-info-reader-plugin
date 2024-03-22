using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.TV;
using YTINFOReader.Helpers;

namespace YTINFOReader;

public class EpisodeProvider : AbstractProvider<EpisodeProvider, Episode, EpisodeInfo>
{
    public EpisodeProvider(IFileSystem fileSystem, ILogger<EpisodeProvider> logger) : base(fileSystem, logger) { }

    public override string Name => $"{Constants.PLUGIN_NAME}: Episode Provider";

    internal override MetadataResult<Episode> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToEpisode(jsonObj);
}
