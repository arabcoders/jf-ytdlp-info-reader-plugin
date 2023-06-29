using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.YTINFOReader.Helpers;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public class LocalSeasonProvider : ILocalMetadataProvider<Season>, IHasItemChangeMonitor
    {
        protected readonly ILogger<LocalSeasonProvider> _logger;
        public string Name => Constants.PLUGIN_NAME;
        public LocalSeasonProvider(ILogger<LocalSeasonProvider> logger)
        {
            _logger = logger;
        }
        public Task<MetadataResult<Season>> GetMetadata(ItemInfo info, IDirectoryService directoryService, CancellationToken cancellationToken)
        {
            _logger.LogDebug("YTLocalSeason GetMetadata: {Path}", info.Path);
            MetadataResult<Season> result = new();
            var item = new Season();
            item.Name = Path.GetFileNameWithoutExtension(info.Path);
            result.Item = item;
            result.HasMetadata = true;
            return Task.FromResult(result);
        }
        public bool HasChanged(BaseItem item, IDirectoryService directoryService)
        {
            return true;
        }
    }
}
