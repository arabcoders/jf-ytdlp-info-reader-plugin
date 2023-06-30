using Jellyfin.Data.Enums;
using MediaBrowser.Model.IO;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Persistence;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.YTINFOReader.Helpers
{
    public class EpisodeIndexer : ILibraryPostScanTask, IScheduledTask
    {
        protected readonly ILibraryManager _libmanager;
        protected readonly IItemRepository _repository;
        protected readonly ILogger<EpisodeIndexer> _logger;
        protected readonly IFileSystem _fileSystem;

        public EpisodeIndexer(
            ILibraryManager libmanager,
            IItemRepository repository,
            IFileSystem fileSystem,
            ILogger<EpisodeIndexer> logger)
        {
            _libmanager = libmanager;
            _repository = repository;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public string Name => Constants.PLUGIN_NAME;

        public string Key => Constants.PLUGIN_NAME;

        public string Description => Constants.PLUGIN_NAME;

        public string Category => Constants.PLUGIN_NAME;

        public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            await Run(progress, cancellationToken);
            return;
        }

        public async Task Execute(IProgress<double> progress, CancellationToken cancellationToken)
        {
            await Run(progress, cancellationToken);
            return;
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[]
            {
                new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerInterval, IntervalTicks = TimeSpan.FromHours(24).Ticks }
            };
        }

        public async Task Run(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting Reindexing episodes");
            var shows = _repository.GetItems(new InternalItemsQuery
            {
                Recursive = true,
                IncludeItemTypes = new[] { BaseItemKind.Series },
                DtoOptions = new DtoOptions()
            });
            var count = 0;
            foreach (var show in shows.Items)
            {
                if (!show.ProviderIds.ContainsKey(Constants.PLUGIN_NAME))
                {
                    _logger.LogDebug("Skipping show {Name}", show.Name);
                    continue;
                }

                _logger.LogDebug("Indexing show {Name}", show.Name);

                var seasons = new List<BaseItem>(_repository.GetItems(new InternalItemsQuery
                {
                    ParentId = show.Id,
                    IncludeItemTypes = new[] { BaseItemKind.Season },
                    DtoOptions = new DtoOptions()
                }).Items);

                seasons.Sort(delegate (BaseItem x, BaseItem y)
                {
                    if (x.Name == null && y.Name == null) return 0;
                    else if (x.Name == null) return -1;
                    else if (y.Name == null) return 1;
                    else return x.Name.CompareTo(y.Name);
                });

                var sindex = 1;

                foreach (var season in seasons)
                {
                    season.IndexNumber = sindex;
                    _logger.LogDebug("Indexing season {Name} as index {Index}", season.Name, sindex);
                    await _libmanager.UpdateItemAsync(season, show, ItemUpdateType.MetadataEdit, cancellationToken);

                    var episodes = new List<BaseItem>(_repository.GetItems(new InternalItemsQuery
                    {
                        AncestorIds = new[] { season.Id },
                        IncludeItemTypes = new[] { BaseItemKind.Episode },
                        DtoOptions = new DtoOptions()
                    }).Items);

                    episodes.Sort(delegate (BaseItem x, BaseItem y)
                    {
                        if (!x.PremiereDate.HasValue && !y.PremiereDate.HasValue)
                        {
                            _logger.LogWarning("Episode [{Name}] does not have 'PremiereDate'", x.FileNameWithoutExtension);
                            _logger.LogWarning("Episode [{Name}] does not have 'PremiereDate'", y.FileNameWithoutExtension);
                            return 0;
                        }
                        else if (!x.PremiereDate.HasValue)
                        {
                            _logger.LogWarning("Episode [{Name}] does not have 'PremiereDate'", x.FileNameWithoutExtension);
                            return -1;
                        }
                        else if (!y.PremiereDate.HasValue)
                        {
                            _logger.LogWarning("Episode [{Name}] does not have 'PremiereDate'", y.FileNameWithoutExtension);
                            return 1;
                        }
                        else
                        {
                            return DateTime.Compare(x.PremiereDate.Value, y.PremiereDate.Value);
                        }
                    });

                    var eindex = 1;
                    foreach (var episode in episodes)
                    {
                        if (episode.PremiereDate.HasValue)
                        {
                            DateTime PremiereDate = episode.PremiereDate ?? DateTime.UtcNow;
                            episode.IndexNumber = int.Parse("1" + PremiereDate.ToString("MMdd") + _fileSystem.GetLastWriteTimeUtc(episode.Path).ToString("hhmm"));
                            _logger.LogDebug("Episode [{Name} - {Date:MM/dd/yyyy}] should now be number {IndexNumber}", episode.Name, episode.PremiereDate, episode.IndexNumber);
                        }
                        else
                        {
                            _logger.LogDebug("Episode [{Name}] has no PremiereDate and should now be index {Index}", episode.Name, eindex);
                            episode.IndexNumber = eindex;
                        }

                        episode.ParentIndexNumber = sindex;
                        await _libmanager.UpdateItemAsync(episode, season, ItemUpdateType.MetadataEdit, cancellationToken);
                        eindex++;
                    }

                    sindex++;
                }

                count++;
                double percent = ((double)count / shows.Items.Count) * 100;
                progress.Report(percent);

            }
            return;
        }
    }
}
