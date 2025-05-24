using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.Movies;
using YTINFOReader.Helpers;

namespace YTINFOReader.Provider;

public class MovieProvider : AbstractProvider<MovieProvider, Movie, MovieInfo>
{
    public MovieProvider(IFileSystem fileSystem, ILogger<MovieProvider> logger) : base(fileSystem, logger) { }
    public override string Name => $"{Constants.PLUGIN_NAME}: Movie Provider";

    internal override MetadataResult<Movie> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToMovie(jsonObj);
}
