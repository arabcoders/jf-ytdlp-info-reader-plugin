using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YTINFOReader.Helpers;

namespace YTINFOReader;

public class SeriesProvider : AbstractProvider<SeriesProvider, Series, SeriesInfo>, IHasItemChangeMonitor
{
    private readonly Matcher _matcher = new();

    public SeriesProvider(IFileSystem fileSystem, ILogger<SeriesProvider> logger) : base(fileSystem, logger)
    {
        _matcher.AddInclude("**/*.info.json");
    }

    public override string Name => $"{Constants.PLUGIN_NAME}: Series Provider";

    internal override MetadataResult<Series> GetMetadataImpl(YTDLData jsonObj) => Utils.YTDLJsonToSeries(jsonObj);

    private string GetSeriesInfo(string path)
    {
        _logger.LogDebug($"{Name} GetSeriesInfo: '{path}'.");

        string infoPath = "";

        foreach (string file in _matcher.GetResultsInFullPath(path))
        {
            if (Utils.RX_C.IsMatch(file) || Utils.RX_P.IsMatch(file))
            {
                infoPath = file;
                break;
            }
        }

        _logger.LogDebug($"{Name} GetSeriesInfo Result: '{infoPath}'.");

        return infoPath;
    }

    public override Task<MetadataResult<Series>> GetMetadata(SeriesInfo info, CancellationToken cancellationToken)
    {
        MetadataResult<Series> result = new();

        if (!Utils.IsYouTubeContent(info.Path))
        {
            _logger.LogDebug($"{Name} GetMetadata: not a youtube series. '{info.Path}'.");
            return Task.FromResult(result);
        }

        string infoPath = GetSeriesInfo(info.Path);
        if (string.IsNullOrEmpty(infoPath))
        {
            return Task.FromResult(result);
        }

        var infoJson = Utils.ReadYTDLInfo(infoPath, _fileSystem.GetFileSystemInfo(info.Path), cancellationToken);
        result = Utils.YTDLJsonToSeries(infoJson);

        _logger.LogDebug($"{Name} GetMetadata Result: '{result}'.");

        return Task.FromResult(result);
    }

    FileSystemMetadata GetInfoJson(string path)
    {
        var fileInfo = _fileSystem.GetFileSystemInfo(path);
        var directoryInfo = fileInfo.IsDirectory ? fileInfo : _fileSystem.GetDirectoryInfo(Path.GetDirectoryName(path));
        var directoryPath = directoryInfo.FullName;
        var specificFile = Path.Combine(directoryPath, Path.GetFileNameWithoutExtension(path) + ".info.json");
        var file = _fileSystem.GetFileInfo(specificFile);
        return file;
    }

    public override bool HasChanged(BaseItem item, IDirectoryService directoryService)
    {
        _logger.LogDebug($"{Name} HasChanged: '{item.Path}'.");

        var infoPath = GetSeriesInfo(item.Path);
        var result = false;
        if (!string.IsNullOrEmpty(infoPath))
        {
            var infoJson = GetInfoJson(infoPath);
            result = infoJson.Exists && infoJson.LastWriteTimeUtc.ToUniversalTime() > item.DateLastSaved.ToUniversalTime();
        }

        string status = result ? "Has Changed" : "Has Not Changed";

        _logger.LogDebug($"{Name} HasChanged Result: '{status}'.");

        return result;
    }

}
