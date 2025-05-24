using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YTINFOReader.Helpers;
using MediaBrowser.Model.IO;

namespace YTINFOReader.Provider;

public class SeasonProvider : AbstractProvider<SeasonProvider, Season, SeasonInfo>
{
    public override string Name => $"{Constants.PLUGIN_NAME}: Season Provider";

    public SeasonProvider(IFileSystem fileSystem, ILogger<SeasonProvider> logger) : base(fileSystem, logger)
    {
        Utils.Logger = logger;
    }

    public override Task<MetadataResult<Season>> GetMetadata(SeasonInfo info, CancellationToken cancellationToken)
    {
        MetadataResult<Season> result = new();

        if (!Utils.IsYouTubeContent(info.Path))
        {
            _logger.LogDebug($"{Name} GetMetadata: is not youtube content '{info.Path}'.");
            return Task.FromResult(result);
        }

        _logger.LogDebug($"{Name} GetMetadata: '{info.Path}'.");

        result.Item = new Season { Name = Path.GetFileNameWithoutExtension(info.Path) };
        result.HasMetadata = true;

        return Task.FromResult(result);
    }

    internal override MetadataResult<Season> GetMetadataImpl(YTDLData jsonObj) => throw new System.NotImplementedException();
}
