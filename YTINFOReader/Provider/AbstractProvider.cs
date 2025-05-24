using YTINFOReader.Helpers;
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

namespace YTINFOReader.Provider;

public abstract class AbstractProvider<B, T, E> : IRemoteMetadataProvider<T, E>, IHasItemChangeMonitor
    where T : BaseItem, IHasLookupInfo<E>
    where E : ItemLookupInfo, new()
{
    protected readonly ILogger<B> _logger;
    protected readonly IFileSystem _fileSystem;

    public AbstractProvider(IFileSystem fileSystem, ILogger<B> logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
        Utils.Logger = logger;
    }

    public abstract string Name { get; }

    public virtual Task<MetadataResult<T>> GetMetadata(E info, CancellationToken cancellationToken)
    {
        MetadataResult<T> result = new();

        if (!Utils.IsYouTubeContent(info.Path))
        {
            _logger.LogDebug($"{Name} GetMetadata: is not youtube content '{info.Path}'.");
            return Task.FromResult(result);
        }

        _logger.LogDebug($"{Name} GetMetadata: '{info.Path}'.");

        var infoFile = Path.ChangeExtension(info.Path, "info.json");

        if (!File.Exists(infoFile))
        {
            _logger.LogError($"{Name} GetMetadata: No json file was found for '{info.Path}'.");
            return Task.FromResult(result);
        }

        var jsonObj = Utils.ReadYTDLInfo(infoFile, _fileSystem.GetFileSystemInfo(info.Path), cancellationToken);

        _logger.LogDebug($"{Name} GetMetadata Result: '{jsonObj}'.");

        return Task.FromResult(GetMetadataImpl(jsonObj));
    }

    public virtual bool HasChanged(BaseItem item, IDirectoryService directoryService)
    {
        _logger.LogDebug($"{Name} HasChanged: '{item.Path}'.");

        var infoFile = Path.ChangeExtension(item.Path, "info.json");

        if (!File.Exists(infoFile))
        {
            _logger.LogDebug($"{Name} HasChanged: No json file was found for '{item.Path}'.");
            return false;
        }

        FileSystemMetadata infoJson = directoryService.GetFile(infoFile);
        bool result = infoJson.Exists && infoJson.LastWriteTimeUtc.ToUniversalTime() > item.DateLastSaved.ToUniversalTime();
        string status = result ? "Has Changed" : "Has Not Changed";

        _logger.LogDebug($"{Name} HasChanged Result: '{status}'.");

        return result;
    }

    internal abstract MetadataResult<T> GetMetadataImpl(YTDLData jsonObj);
    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(E searchInfo, CancellationToken cancellationToken) => throw new NotImplementedException();
    public virtual Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => throw new NotImplementedException();
}
