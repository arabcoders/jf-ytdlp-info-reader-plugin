﻿using Xunit;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using Newtonsoft.Json;
using MediaBrowser.Controller;
using Moq;
using System.Collections.Generic;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Controller.Entities.Movies;
using System;
using System.IO;
using Jellyfin.Plugin.YTINFOReader.Helpers;
using System.Text.RegularExpressions;

namespace Jellyfin.Plugin.YTINFOReader.Tests
{
    public class UtilsTest
    {
        [Theory]
        [InlineData("Foo", "")]
        [InlineData("ChannelName - 20190113 - this is a test title [dQw4w9WgXcQ].mkv", "dQw4w9WgXcQ")]
        [InlineData("ChannelName - 20190113 - this is a test title [youtube-dQw4w9WgXcQ].mkv", "dQw4w9WgXcQ")]
        public void GetYouTubeChannelOrVideoIds(string fn, string expected)
        {
            var rxc = new Regex(Constants.VIDEO_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var result = "";
            if (rxc.IsMatch(fn))
            {
                MatchCollection match = rxc.Matches(fn);
                result = match[0].Groups["id"].ToString();
            }

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Foo", "")]
        [InlineData("ChannelName Videos [UCuAXFkgsw1L7xaCfnd5JJOw].info.json", "UCuAXFkgsw1L7xaCfnd5JJOw")]
        [InlineData("ChannelName Videos [youtube-UCuAXFkgsw1L7xaCfnd5JJOw].info.json", "UCuAXFkgsw1L7xaCfnd5JJOw")]
        [InlineData("ChannelName Videos [youtube-HCuAXFkgsw1L7xaCfnd5JJOw].info.json", "HCuAXFkgsw1L7xaCfnd5JJOw")]
        public void GetYouTubeChannelIds(string fn, string expected)
        {
            var rxc = new Regex(Constants.CHANNEL_RX, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var result = "";
            if (rxc.IsMatch(fn))
            {
                MatchCollection match = rxc.Matches(fn);
                result = match[0].Groups["id"].ToString();
            }

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreatePersonTest()
        {
            var result = Utils.CreatePerson("Rick Astley", "UCuAXFkgsw1L7xaCfnd5JJOw");
            var expected = new PersonInfo
            {
                Name = "Rick Astley",
                Type = PersonType.Director,
                ProviderIds = new Dictionary<string, string> { { Constants.PLUGIN_NAME, "UCuAXFkgsw1L7xaCfnd5JJOw" } }
            };

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public void GetVideoInfoPathTest()
        {
            var mockAppPath = Mock.Of<IServerApplicationPaths>(a =>
                a.CachePath == Path.Combine("foo", "bar").ToString()
            );

            var result = Utils.GetVideoInfoPath(mockAppPath, "id123");
            Assert.Equal(Path.Combine("foo", "bar", Constants.PLUGIN_NAME, "id123", "ytvideo.info.json").ToString(), result);
        }

        [Fact]
        public void YTDLJsonToMovieTest()
        {
            var result = Utils.YTDLJsonToMovie(GetYouTubeVideoData());

            Assert.True(result.HasMetadata);
            Assert.Equal("Never Gonna Give You Up", result.Item.Name);
            Assert.Equal("The official video for “Never Gonna Give You Up” by Rick Astley", result.Item.Overview);
            Assert.Equal(2009, result.Item.ProductionYear);
            Assert.Equal("20091025", (result.Item.PremiereDate ?? DateTime.Now).ToString("yyyyMMdd"));
            Assert.Equal("Rick Astley", result.People[0].Name);
            Assert.Equal("UCuAXFkgsw1L7xaCfnd5JJOw", result.People[0].ProviderIds[Constants.PLUGIN_NAME]);
            Assert.Equal("dQw4w9WgXcQ", result.Item.ProviderIds[Constants.PLUGIN_NAME]);
        }

        [Fact]
        public void YTDLJsonToMusicTest()
        {
            var result = Utils.YTDLJsonToMusicVideo(GetYouTubeVideoData());
            Assert.True(result.HasMetadata);
            Assert.Equal("Music", result.Item.Name);
            Assert.Equal("The official video for “Never Gonna Give You Up” by Rick Astley", result.Item.Overview);
            Assert.Equal(2009, result.Item.ProductionYear);
            Assert.Equal("20091025", (result.Item.PremiereDate ?? DateTime.Now).ToString("yyyyMMdd"));
            Assert.Equal("Rick Astley", result.People[0].Name);
            Assert.Equal("UCuAXFkgsw1L7xaCfnd5JJOw", result.People[0].ProviderIds[Constants.PLUGIN_NAME]);
            Assert.Equal("dQw4w9WgXcQ", result.Item.ProviderIds[Constants.PLUGIN_NAME]);
        }

        [Fact]
        public void YTDLJsonToEpisodeTest()
        {
            var result = Utils.YTDLJsonToEpisode(GetYouTubeVideoData());
            Assert.True(result.HasMetadata);
            Assert.Equal("Never Gonna Give You Up", result.Item.Name);
            Assert.Equal("The official video for “Never Gonna Give You Up” by Rick Astley", result.Item.Overview);
            Assert.Equal(2009, result.Item.ProductionYear);
            Assert.Equal("20091025", (result.Item.PremiereDate ?? DateTime.Now).ToString("yyyyMMdd"));
            Assert.Equal("Rick Astley", result.People[0].Name);
            Assert.Equal("UCuAXFkgsw1L7xaCfnd5JJOw", result.People[0].ProviderIds[Constants.PLUGIN_NAME]);
            Assert.Equal("20091025-Never Gonna Give You Up", result.Item.ForcedSortName);
            Assert.Equal(11025, result.Item.IndexNumber);
            Assert.Equal(2009, result.Item.ParentIndexNumber);
            Assert.Equal("dQw4w9WgXcQ", result.Item.ProviderIds[Constants.PLUGIN_NAME]);
        }

        [Fact]
        public void YTDLJsonToSeriesTest()
        {
            var result = Utils.YTDLJsonToSeries(GetYouTubeChannelData());

            Assert.True(result.HasMetadata);
            Assert.Equal("Rick Astley", result.Item.Name);
            Assert.Equal("Official YouTube channel for Rick Astley.", result.Item.Overview);
            Assert.Equal("UCuAXFkgsw1L7xaCfnd5JJOw", result.Item.ProviderIds[Constants.PLUGIN_NAME]);
        }

        public static YTDLData GetYouTubeVideoData()
        {
            return new YTDLData
            {
                id = "dQw4w9WgXcQ",
                title = "Never Gonna Give You Up",
                track = "Music",
                artist = "Rick Astley",
                description = "The official video for “Never Gonna Give You Up” by Rick Astley",
                upload_date = "20091025",
                uploader = "Rick Astley",
                channel_id = "UCuAXFkgsw1L7xaCfnd5JJOw"
            };
        }

        public static YTDLData GetYouTubeChannelData()
        {
            return new YTDLData
            {
                id = "UCuAXFkgsw1L7xaCfnd5JJOw",
                title = "Rick Astley",
                description = "Official YouTube channel for Rick Astley.",
                uploader = "Rick Astley",
                channel_id = "UCuAXFkgsw1L7xaCfnd5JJOw"
            };
        }
    }
}
