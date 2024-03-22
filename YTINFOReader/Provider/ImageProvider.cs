using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.TV;
using Microsoft.Extensions.FileSystemGlobbing;
using YTINFOReader.Helpers;
using MediaBrowser.Model.Entities;
using System.Threading.Tasks;
using MediaBrowser.Model.Providers;
using System.Threading;
using System.Net.Http;
using System.IO;

namespace YTINFOReader;

public class ImageProvider : IRemoteImageProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<ImageProvider> _logger;
    private readonly string[] _supportedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly Matcher _matcher = new();

    public string Name => $"{Constants.PLUGIN_NAME}: Image Provider";

    public ImageProvider(IFileSystem fileSystem, ILogger<ImageProvider> logger)
    {
        _logger = logger;
        Utils.Logger = logger;
        _fileSystem = fileSystem;

        for (int i = 0; i < _supportedExtensions.Length; i++)
        {
            _matcher.AddInclude($"**/*{_supportedExtensions[i]}");
        }

    }

    public bool Supports(BaseItem item) => item is Series || item is Episode;

    private string GetInfo(BaseItem item)
    {
        var path = item is Episode ? Path.GetDirectoryName(item.Path) : item.Path;

        _logger.LogDebug($"{Name} GetInfo: '{path}'.");

        if (string.IsNullOrEmpty(path))
        {
            return "";
        }

        string infoPath = "";

        foreach (string file in _matcher.GetResultsInFullPath(path))
        {
            switch (item)
            {
                default:
                    break;
                case Series:
                    if (Utils.RX_C.IsMatch(file) || Utils.RX_P.IsMatch(file))
                    {
                        infoPath = file;
                    }
                    break;
                case Episode:
                    if (Utils.RX_V.IsMatch(file))
                    {
                        infoPath = file;
                    }
                    break;
            }
        }

        _logger.LogDebug($"{Name} GetInfo Result: '{infoPath}'.");

        return infoPath;
    }

    /// <summary>
    /// Retrieves a list of local image information for the specified item.
    /// </summary>
    /// <param name="item">The item to retrieve images for.</param>
    /// <param name="directoryService">The directory service to use for retrieving images.</param>
    /// <returns>A list of local image information for the specified item.</returns>
    public IEnumerable<LocalImageInfo> GetImages(BaseItem item, IDirectoryService directoryService)
    {
        _logger.LogDebug($"{Name} GetImages: {item.Name}");
        var list = new List<LocalImageInfo>();

        if (!Utils.IsYouTubeContent(item.Path))
        {
            return list;
        }

        string jpgPath = GetInfo(item);
        if (string.IsNullOrEmpty(jpgPath))
        {
            return list;
        }

        var fileInfo = _fileSystem.GetFileSystemInfo(jpgPath);
        list.Add(new LocalImageInfo { FileInfo = fileInfo });

        _logger.LogDebug($"{Name} GetImages Result: {list}");

        return list;
    }

    public IEnumerable<ImageType> GetSupportedImages(BaseItem item) => new List<ImageType> { ImageType.Primary };

    public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken) => throw new System.NotImplementedException();

    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => throw new System.NotImplementedException();
}
