using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging;
using MediaBrowser.Controller.Entities.TV;
using Microsoft.Extensions.FileSystemGlobbing;
using Jellyfin.Plugin.YTINFOReader.Helpers;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public class ImageSeriesProvider : ILocalImageProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<ImageSeriesProvider> _logger;
        public string Name => Constants.PLUGIN_NAME;

        public ImageSeriesProvider(IFileSystem fileSystem, ILogger<ImageSeriesProvider> logger)
        {
            _logger = logger;
            Utils.Logger = logger;
            _fileSystem = fileSystem;
        }
        public bool Supports(BaseItem item) => item is Series;

        private string GetSeriesInfo(string path)
        {
            _logger.LogDebug("YTIR Series Image GetSeriesInfo: {Path}", path);
            Matcher matcher = new();
            matcher.AddInclude("**/*.jpg");
            matcher.AddInclude("**/*.png");
            matcher.AddInclude("**/*.webp");
            string infoPath = "";
            foreach (string file in matcher.GetResultsInFullPath(path))
            {
                if (Utils.RX_C.IsMatch(file) || Utils.RX_P.IsMatch(file))
                {
                    infoPath = file;
                    break;
                }
            }
            _logger.LogDebug("YTIR Series Image GetSeriesInfo Result: {InfoPath}", infoPath);
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
            _logger.LogDebug("YTIR Series Image GetImages: {Name}", item.Name);
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
            _logger.LogDebug("YTIR Series Image GetImages Result: {Result}", list.ToString());
            return list;
        }

    }
}
