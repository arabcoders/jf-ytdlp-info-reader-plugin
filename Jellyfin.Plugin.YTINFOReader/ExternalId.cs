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
        public bool Supports(IHasProviderIds item) => item is Movie || item is Episode || item is MusicVideo;
        public string ProviderName => Constants.PLUGIN_NAME;
        public string Key => Constants.PLUGIN_NAME;
        public ExternalIdMediaType? Type => null;
        public string UrlFormatString => Constants.VIDEO_URL;
    }

    public class SeriesExternalId : IExternalId
    {
        public bool Supports(IHasProviderIds item)
        {
            if (false == item.ProviderIds.TryGetValue(Constants.PLUGIN_NAME, out var id))
            {
                return false;
            }

            var isChannel = id.StartsWith("UC") || id.StartsWith("HC");

            return item is Series && isChannel;
        }

        public string ProviderName => Constants.PLUGIN_NAME;
        public string Key => Constants.PLUGIN_NAME;
        public ExternalIdMediaType? Type => ExternalIdMediaType.Series;
        public string UrlFormatString => Constants.CHANNEL_URL;
    }

    public class PlaylistExternalId : IExternalId
    {
        public bool Supports(IHasProviderIds item)
        {
            if (false == item.ProviderIds.TryGetValue(Constants.PLUGIN_NAME, out var id))
            {
                return false;
            }

            var isPlaylist = id.StartsWith("PL") || id.StartsWith("UU") || id.StartsWith("FL") || id.StartsWith("LP") || id.StartsWith("RD");

            return item is Series && isPlaylist;
        }

        public string ProviderName => Constants.PLUGIN_NAME;
        public string Key => Constants.PLUGIN_NAME;
        public ExternalIdMediaType? Type => ExternalIdMediaType.Series;
        public string UrlFormatString => Constants.PLAYLIST_URL;
    }

    public class PlaylistSeasonExternalId : IExternalId
    {
        public bool Supports(IHasProviderIds item)
        {
            if (false == item.ProviderIds.TryGetValue(Constants.PLUGIN_NAME, out var id))
            {
                return false;
            }

            var isPlaylist = id.StartsWith("PL") || id.StartsWith("UU") || id.StartsWith("FL") || id.StartsWith("LP") || id.StartsWith("RD");

            return item is Season && isPlaylist;
        }
        public string ProviderName => Constants.PLUGIN_NAME;
        public string Key => Constants.PLUGIN_NAME;
        public ExternalIdMediaType? Type => ExternalIdMediaType.Season;
        public string UrlFormatString => Constants.PLAYLIST_URL;
    }

}
