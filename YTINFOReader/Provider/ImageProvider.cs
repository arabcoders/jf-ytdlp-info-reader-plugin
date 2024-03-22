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

namespace YTINFOReader;

public class ImageProvider : IRemoteImageProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<ImageProvider> _logger;
    private readonly string[] _supportedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    public string Name => Constants.PLUGIN_NAME;

    public ImageProvider(IFileSystem fileSystem, ILogger<ImageProvider> logger)
    {
        _logger = logger;
        Utils.Logger = logger;
        _fileSystem = fileSystem;
    }

    public bool Supports(BaseItem item) => item is Series || item is Episode;

    private string GetSeriesInfo(string path)
    {
        _logger.LogDebug("YIR Series Image GetSeriesInfo: {Path}", path);
        Matcher matcher = new();
        for (int i = 0; i < _supportedExtensions.Length; i++)
        {
            matcher.AddInclude($"**/*{_supportedExtensions[i]}");
        }

        string infoPath = "";
        foreach (string file in matcher.GetResultsInFullPath(path))
        {
            if (Utils.RX_C.IsMatch(file) || Utils.RX_P.IsMatch(file))
            {
                infoPath = file;
                break;
            }
        }
        _logger.LogDebug("YIR Series Image GetSeriesInfo Result: {InfoPath}", infoPath);
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
        _logger.LogDebug("YIR Series Image GetImages: {Name}", item.Name);
        var list = new List<LocalImageInfo>();

        if (!Utils.IsYouTubeContent(item.Path))
        {
            return list;
        }

        string jpgPath = GetSeriesInfo(item.Path);
        if (string.IsNullOrEmpty(jpgPath))
        {
            return list;
        }
        var localImg = new LocalImageInfo();
        var fileInfo = _fileSystem.GetFileSystemInfo(jpgPath);
        localImg.FileInfo = fileInfo;
        list.Add(localImg);
        _logger.LogDebug("YIR Series Image GetImages Result: {Result}", list.ToString());
        return list;
    }

    public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
    {
        return new List<ImageType> { ImageType.Primary };
    }

    public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
