using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Jellyfin.Data.Enums;

namespace YTINFOReader.Helpers;

public class Utils
{
    public static readonly Regex RX_C = new(Constants.CHANNEL_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex RX_P = new(Constants.PLAYLIST_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex RX_V = new(Constants.VIDEO_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly JsonSerializerOptions JSON_OPTS = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
    };

#nullable enable
    public static ILogger? Logger { get; set; }
#nullable disable

    public static bool IsFresh(FileSystemMetadata fileInfo)
    {
        if (fileInfo.Exists && DateTime.UtcNow.Subtract(fileInfo.LastWriteTimeUtc).Days <= 10)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns boolean if the given content is youtube.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool IsYouTubeContent(string name)
    {
        return RX_C.IsMatch(name) || RX_P.IsMatch(name) || RX_V.IsMatch(name);
    }

    /// <summary>
    ///  Returns the Youtube ID from the file path. Matches last 11 character field inside square brackets.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetYTID(string name)
    {
        if (RX_C.IsMatch(name))
        {
            MatchCollection match = RX_C.Matches(name);
            return match[0].Groups["id"].ToString();
        }

        if (RX_P.IsMatch(name))
        {
            MatchCollection match = RX_P.Matches(name);
            return match[0].Groups["id"].ToString();
        }

        if (RX_V.IsMatch(name))
        {
            MatchCollection match = RX_V.Matches(name);
            return match[0].Groups["id"].ToString();
        }

        return "";
    }

    /// <summary>
    /// Creates a person object of type director for the provided name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="channel_id"></param>
    /// <returns></returns>
    public static PersonInfo CreatePerson(string name, string channel_id)
    {
        return new PersonInfo
        {
            Name = name,
            Type = PersonKind.Director,
            ProviderIds = new Dictionary<string, string> { { Constants.PLUGIN_NAME, channel_id } },
        };
    }

    /// <summary>
    /// Returns path to where metadata json file should be.
    /// </summary>
    /// <param name="appPaths"></param>
    /// <param name="youtubeID"></param>
    /// <returns></returns>
    public static string GetVideoInfoPath(IServerApplicationPaths appPaths, string youtubeID)
    {
        var dataPath = Path.Combine(appPaths.CachePath, Constants.PLUGIN_NAME, youtubeID);
        return Path.Combine(dataPath, "ytvideo.info.json");
    }

    /// <summary>
    /// Reads JSON data from file.
    /// </summary>
    /// <param name="metaFile"></param>
    /// <param name="FileSystemMetadata"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static YTDLData ReadYTDLInfo(string fpath, FileSystemMetadata path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string jsonString = File.ReadAllText(fpath);

        YTDLData data = JsonSerializer.Deserialize<YTDLData>(jsonString, JSON_OPTS);

        data.File_path = path;
        return data;
    }

    /// <summary>
    /// Provides a Movie Metadata Result from a json object.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MetadataResult<Movie> YTDLJsonToMovie(YTDLData json, string name = "")
    {
        Logger?.LogDebug($"{name} Processing: '{json}'.");

        var item = new Movie();
        var result = new MetadataResult<Movie>
        {
            HasMetadata = true,
            Item = item
        };
        result.Item.Name = json.Title.Trim();
        result.Item.Overview = json.Description.Trim();
        var date = new DateTime(1970, 1, 1);
        try
        {
            date = DateTime.ParseExact(json.Upload_date, "yyyyMMdd", null);
        }
        catch { }
        result.Item.ProductionYear = date.Year;
        result.Item.PremiereDate = date;
        result.AddPerson(CreatePerson(json.Uploader.Trim(), json.Channel_id));
        result.Item.ProviderIds.Add(Constants.PLUGIN_NAME, json.Id);

        return result;
    }

    /// <summary>
    /// Provides a MusicVideo Metadata Result from a json object.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MetadataResult<MusicVideo> YTDLJsonToMusicVideo(YTDLData json, string name = "")
    {
        Logger?.LogDebug($"{name} processing: '{json}'.");
        var item = new MusicVideo();
        var result = new MetadataResult<MusicVideo>
        {
            HasMetadata = true,
            Item = item
        };
        result.Item.Name = string.IsNullOrEmpty(json.Track) ? json.Title.Trim() : json.Track.Trim();
        result.Item.Artists = new List<string> { !string.IsNullOrEmpty(json.Artist) ? json.Artist : json.Channel };
        if (!string.IsNullOrEmpty(json.Album))
        {
            result.Item.Album = json.Album;
        }

        if (!string.IsNullOrEmpty(json.Track))
        {
            result.Item.Name = json.Track;
        }

        result.Item.Overview = json.Description.Trim();
        var date = new DateTime(1970, 1, 1);
        try
        {
            date = DateTime.ParseExact(json.Upload_date, "yyyyMMdd", null);
        }
        catch { }
        result.Item.ProductionYear = date.Year;
        result.Item.PremiereDate = date;
        result.AddPerson(CreatePerson(json.Uploader.Trim(), json.Channel_id));
        result.Item.ProviderIds.Add(Constants.PLUGIN_NAME, json.Id);

        return result;
    }

    /// <summary>
    /// Provides a Episode Metadata Result from a json object.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MetadataResult<Episode> YTDLJsonToEpisode(YTDLData json, string name = "")
    {
        Logger?.LogDebug($"{name} Processing: '{json}'.");
        var item = new Episode();
        var result = new MetadataResult<Episode>
        {
            HasMetadata = true,
            Item = item
        };

        result.Item.Name = json.Title.Trim();
        result.Item.Overview = json.Description.Trim();
        var date = new DateTime(1970, 1, 1);
        try
        {
            date = DateTime.ParseExact(json.Upload_date, "yyyyMMdd", null);
        }
        catch { }
        result.Item.ProductionYear = date.Year;
        result.Item.PremiereDate = date;
        result.Item.ForcedSortName = date.ToString("yyyyMMdd") + "-" + result.Item.Name;
        result.AddPerson(CreatePerson(json.Uploader.Trim(), json.Channel_id));
        result.Item.IndexNumber = int.Parse("1" + date.ToString("MMdd"));
        result.Item.ParentIndexNumber = int.Parse(date.ToString("yyyyMM"));
        result.Item.ProviderIds.Add(Constants.PLUGIN_NAME, json.Id);


        // if no file was found, use epoch time.
        if (json.Epoch != null)
        {
            Logger?.LogDebug($"{name} Using epoch for episode index number for '{json.Id}' - '{json.Title}'.");
            result.Item.IndexNumber = int.Parse("1" + date.ToString("MMdd") + DateTimeOffset.FromUnixTimeSeconds(json.Epoch ?? new long()).ToString("mmss"));
        }

        // append file last write time to index number if available.
        if (json.Epoch == null && json.File_path != null)
        {
            Logger?.LogDebug($"{name} Using file last write time for episode index number for '{json.Id}'- '{json.Title}'.");
            result.Item.IndexNumber = int.Parse("1" + date.ToString("MMdd") + json.File_path.LastWriteTimeUtc.ToString("mmss"));
        }

        if (json.File_path == null && json.Epoch == null)
        {
            Logger?.LogError($"{name} No file or epoch data found for '{json.Id}' - '{json.Title}'.");
        }

        return result;
    }

    /// <summary>
    /// Provides a MusicVideo Metadata Result from a json object.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static MetadataResult<Series> YTDLJsonToSeries(YTDLData json, string name = "")
    {
        Logger?.LogDebug($"{name} Processing: '{json}'.");
        var item = new Series();
        var result = new MetadataResult<Series>
        {
            HasMetadata = true,
            Item = item
        };

        var identifier = json.Channel_id;
        var nameEx = "[" + json.Id + "]";
        result.Item.Name = json.Title.Trim();
        result.Item.Overview = json.Description.Trim();

        if (RX_C.IsMatch(nameEx))
        {
            MatchCollection match = RX_C.Matches(nameEx);
            identifier = match[0].Groups["id"].ToString();
        }
        else
        {
            if (RX_P.IsMatch(nameEx))
            {
                MatchCollection match = RX_P.Matches(nameEx);
                identifier = match[0].Groups["id"].ToString();
            }
        }

        result.Item.ProviderIds.Add(Constants.PLUGIN_NAME, identifier);
        return result;
    }
}
