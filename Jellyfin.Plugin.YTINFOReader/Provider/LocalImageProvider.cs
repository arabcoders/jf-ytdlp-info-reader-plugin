using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Controller.Entities.Movies;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;
using System;
using MediaBrowser.Controller.Entities.TV;
using Jellyfin.Plugin.YTINFOReader.Helpers;

namespace Jellyfin.Plugin.YTINFOReader.Provider
{
    public class LocalImageProvider : ILocalImageProvider, IHasOrder
    {

        public string Name => Constants.PLUGIN_NAME;
        public int Order => 1;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<LocalImageProvider> _logger;

        public LocalImageProvider(IFileSystem fileSystem, ILogger<LocalImageProvider> logger)
        {
            _logger = logger;
            Utils.Logger = logger;
            _fileSystem = fileSystem;
        }
        public bool Supports(BaseItem item) => item is Movie || item is Episode || item is MusicVideo;

        private string GetSeriesInfo(string path)
        {
            _logger.LogDebug("YTLocalImage GetSeriesInfo: {Path}", path);
            Matcher matcher = new();
            Regex rx = new Regex(Constants.VIDEO_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            matcher.AddInclude("*.jpg");
            matcher.AddInclude("*.png");
            matcher.AddInclude("*.webp");
            string infoPath = "";
            foreach (string file in matcher.GetResultsInFullPath(path))
            {
                if (rx.IsMatch(file))
                {
                    infoPath = file;
                    break;
                }
            }
            _logger.LogDebug("YTLocalImage GetSeriesInfo Result: {InfoPath}", infoPath);
            return infoPath;
        }
        /// <summary>
        /// Retrieves Image.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="directoryService"></param>
        /// <returns></returns>
        public IEnumerable<LocalImageInfo> GetImages(BaseItem item, IDirectoryService directoryService)
        {
            _logger.LogDebug("YTLocalImage GetImages: {Name}", item.Name);
            var list = new List<LocalImageInfo>();
            string jpgPath = GetSeriesInfo(item.ContainingFolderPath);
            if (string.IsNullOrEmpty(jpgPath))
            {
                return list;
            }
            var localImg = new LocalImageInfo();
            var fileInfo = _fileSystem.GetFileSystemInfo(jpgPath);
            localImg.FileInfo = fileInfo;
            list.Add(localImg);
            return list;
        }
    }
}
