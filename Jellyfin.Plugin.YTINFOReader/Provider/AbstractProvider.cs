using Jellyfin.Plugin.YTINFOReader.Helpers;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public abstract class AbstractProvider<B, T, E> : IRemoteMetadataProvider<T, E>
        where T : BaseItem, IHasLookupInfo<E>
        where E : ItemLookupInfo, new()
    {
        protected readonly IServerConfigurationManager _config;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger<B> _logger;
        protected readonly IFileSystem _fileSystem;
        protected readonly System.IO.Abstractions.IFileSystem _afs;

        public AbstractProvider(IFileSystem fileSystem,
            IHttpClientFactory httpClientFactory,
            ILogger<B> logger,
            IServerConfigurationManager config,
            System.IO.Abstractions.IFileSystem afs)
        {
            _config = config;
            _fileSystem = fileSystem;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _afs = afs;
            Utils.Logger = logger;
        }

        public abstract string Name { get; }

        /// <summary>
        /// Retrieves metadata of item.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<MetadataResult<T>> GetMetadata(E info, CancellationToken cancellationToken)
        {
            _logger.LogDebug("YTAP GetMetadata: {Path}", info.Path);
            MetadataResult<T> result = new();

            if (!Utils.IsYouTubeContent(info.Path))
            {
                _logger.LogDebug("YTAP GetMetadata: is not youtube content. [{Path}].", info.Path);
                return Task.FromResult(result);
            }

            var infoFile = Path.ChangeExtension(info.Path, "info.json");

            if (!File.Exists(infoFile))
            {
                return Task.FromResult(result);
            }

            var jsonObj = Utils.ReadYTDLInfo(infoFile, _fileSystem.GetFileSystemInfo(info.Path), cancellationToken);
            _logger.LogDebug("YTAP GetMetadata Result: {JSON}", jsonObj.ToString());

            return Task.FromResult(GetMetadataImpl(jsonObj));
        }

        internal abstract MetadataResult<T> GetMetadataImpl(YTDLData jsonObj);

        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(E searchInfo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
