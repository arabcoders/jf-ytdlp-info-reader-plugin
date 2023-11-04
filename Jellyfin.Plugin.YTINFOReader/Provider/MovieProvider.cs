using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.Movies;
using Jellyfin.Plugin.YTINFOReader.Helpers;
using MediaBrowser.Controller.Configuration;
using System.Net.Http;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public class MovieProvider : AbstractProvider<MovieProvider, Movie, MovieInfo>
    {
        public MovieProvider(IFileSystem fileSystem, ILogger<MovieProvider> logger) : base(fileSystem, logger) { }
        public override string Name => Constants.PLUGIN_NAME;
        internal override MetadataResult<Movie> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToMovie(jsonObj);

    }
}
