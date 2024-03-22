using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YTINFOReader.Helpers;

namespace YTINFOReader;

public class LocalSeasonProvider : ILocalMetadataProvider<Season>
{
    protected readonly ILogger<LocalSeasonProvider> _logger;
    public string Name => Constants.PLUGIN_NAME;
    public LocalSeasonProvider(ILogger<LocalSeasonProvider> logger)
    {
        _logger = logger;
        Utils.Logger = logger;
    }
    public Task<MetadataResult<Season>> GetMetadata(ItemInfo info, IDirectoryService directoryService, CancellationToken cancellationToken)
    {
        MetadataResult<Season> result = new();

        if (!Utils.IsYouTubeContent(info.Path))
        {
            _logger.LogDebug("YIR Season GetMetadata: is not youtube content [{Path}].", info.Path);
            return Task.FromResult(result);
        }

        _logger.LogDebug("YIR Season GetMetadata: {Path}", info.Path);

        var item = new Season
        {
            Name = Path.GetFileNameWithoutExtension(info.Path)
        };

        result.Item = item;
        result.HasMetadata = true;

        return Task.FromResult(result);
    }
}
