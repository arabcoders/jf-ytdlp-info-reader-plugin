using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.Movies;
using YTINFOReader.Helpers;

namespace YTINFOReader;

public class MovieProvider : AbstractProvider<MovieProvider, Movie, MovieInfo>
{
    public MovieProvider(IFileSystem fileSystem, ILogger<MovieProvider> logger) : base(fileSystem, logger) { }

    internal override MetadataResult<Movie> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToMovie(jsonObj);
}
