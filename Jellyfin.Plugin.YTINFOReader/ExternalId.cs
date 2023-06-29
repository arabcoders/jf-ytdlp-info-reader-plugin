using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Jellyfin.Plugin.YTINFOReader.Helpers;

namespace Jellyfin.Plugin.YTINFOReader
{
    public class VideoExternalId : IExternalId
    {
        public bool Supports(IHasProviderIds item)
            => item is Movie || item is Episode || item is MusicVideo;

        public string ProviderName => Constants.PLUGIN_NAME;

        public string Key => Constants.PLUGIN_NAME;

        public ExternalIdMediaType? Type => null;

        public string UrlFormatString => Constants.VIDEO_URL;
    }

    public class SeriesExternalId : IExternalId
    {
        public bool Supports(IHasProviderIds item) => item is Series;

        public string ProviderName => Constants.PLUGIN_NAME;

        public string Key => Constants.PLUGIN_NAME;

        public ExternalIdMediaType? Type => null;

        public string UrlFormatString => Constants.CHANNEL_URL;
    }
}
