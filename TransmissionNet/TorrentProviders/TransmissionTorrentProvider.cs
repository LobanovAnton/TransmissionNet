using Transmission.API.RPC;
using Transmission.API.RPC.Params;
using Transmission.API.RPC.Entity;

namespace TransmissionNet.TorrentProviders;

public class TransmissionTorrentProvider(string url, string username, string password) : ITorrentProvider
{
    private const string Provider = "Transmission";

    private static readonly Dictionary<TorrentField, string> FieldMap = new()
    {
        { TorrentField.Id, TorrentFields.ID },
        { TorrentField.Name, TorrentFields.NAME },
        { TorrentField.Availability , TorrentFields.AVAILABILITY },
        { TorrentField.TotalSize, TorrentFields.TOTAL_SIZE },
        { TorrentField.Status, TorrentFields.STATUS },
        { TorrentField.PercentDone, TorrentFields.PERCENT_DONE },
        { TorrentField.RecheckProgress, TorrentFields.RECHECK_PROGRESS },
        { TorrentField.RateDownload, TorrentFields.RATE_DOWNLOAD },
        { TorrentField.RateUpload, TorrentFields.RATE_UPLOAD },
        { TorrentField.UploadRatio, TorrentFields.UPLOAD_RATIO },
        { TorrentField.LeftUntilDone, TorrentFields.LEFT_UNTIL_DONE },
        { TorrentField.SecondsDownloading, TorrentFields.SECONDS_DOWNLOADING },
        { TorrentField.AddedDate, TorrentFields.ADDED_DATE },
        { TorrentField.QueuePosition, TorrentFields.QUEUE_POSITION },
        { TorrentField.DownloadDir, TorrentFields.DOWNLOAD_DIR },
        { TorrentField.Files, TorrentFields.FILES },
        { TorrentField.FileStats, TorrentFields.FILE_STATS },
        { TorrentField.Pieces, TorrentFields.PIECES },
        { TorrentField.PieceCount, TorrentFields.PIECE_COUNT },
        { TorrentField.SequentialDownload, TorrentFields.SEQUENTIAL_DOWNLOAD },
    };

    private static readonly Dictionary<SessionField, string> SessionFieldMap = new()
    {
        { SessionField.SessionId, SessionFields.SESSION_ID },
        { SessionField.Version, SessionFields.VERSION },
        { SessionField.CacheSize, SessionFields.CACHE_SIZE_MB },
        { SessionField.DownloadDir, SessionFields.DOWNLOAD_DIR },
        { SessionField.IncompleteDir, SessionFields.INCOMPLETE_DIR },
        { SessionField.IncompleteDirEnabled, SessionFields.INCOMPLETE_DIR_ENABLED },
        { SessionField.PexEnabled, SessionFields.PEX_ENABLED },
        { SessionField.LpdEnabled, SessionFields.LPD_ENABLED },
        { SessionField.DhtEnabled, SessionFields.DHT_ENABLED },
        { SessionField.UtpEnabled, SessionFields.UTP_ENABLED },
        { SessionField.Encryption, SessionFields.ENCRYPTION },
        { SessionField.Units, SessionFields.UNITS },
        { SessionField.DownloadQueueEnabled, SessionFields.DOWNLOAD_QUEUE_ENABLED },
        { SessionField.DownloadQueueSize, SessionFields.DOWNLOAD_QUEUE_SIZE },
        { SessionField.SeedQueueEnabled, SessionFields.SEED_QUEUE_ENABLED },
        { SessionField.SeedQueueSize, SessionFields.SEED_QUEUE_SIZE },
        { SessionField.PortForwardingEnabled, SessionFields.PORT_FORWARDING_ENABLED },
        { SessionField.PeerPort, SessionFields.PEER_PORT },
        { SessionField.SequentialDownload, SessionFields.SEQUENTIAL_DOWNLOAD },
    };

    private static string? _sessionId;

    private readonly Client _client = new(url, _sessionId, username, password);

    public async Task<long> GetFreeSpaceAsync(string path, CancellationToken cancellationToken = default)
    {
        FreeSpace freeSpace = await _client.FreeSpaceAsync(path, cancellationToken);
        return freeSpace.SizeBytes;
    }

    public Task SetTorrentLocationAsync(int id, string destinationPath, CancellationToken cancellationToken = default)
    {
        return _client.TorrentSetLocationAsync([id], destinationPath, true, cancellationToken);
    }

    public async Task<SessionModel> GetSessionModelAsync(SessionField[] fields, CancellationToken cancellationToken = default)
    {
        HashSet<SessionField> fieldSet = new(fields) { SessionField.SessionId };
        string[] rpcFields = fieldSet.Select(f => SessionFieldMap[f]).ToArray();

        SessionInfo sessionInfo = await _client.GetSessionInformationAsync(rpcFields, cancellationToken);

        _sessionId = sessionInfo.SessionId;

        SessionModel model = new SessionModel
        {
            Version = $"{Provider} {sessionInfo.Version}",
            CacheSize = sessionInfo.CacheSizeMb,
            CompletePath = sessionInfo.DownloadDirectory,
            InCompletePathEnabled = sessionInfo.IncompleteDirectoryEnabled,
            InCompletePath = sessionInfo.IncompleteDirectory,
            UsePex = sessionInfo.PexEnabled,
            UseLpd = sessionInfo.LpdEnabled,
            UseDht = sessionInfo.DhtEnabled,
            UseUdp = sessionInfo.UtpEnabled,
            EncryptionMode = sessionInfo.Encryption != null ?
                Enum.Parse<Encryption>(sessionInfo.Encryption, true)
                : Encryption.Preferred,
            SpeedUnits = sessionInfo.Units?.SpeedBytes ?? 0,
            SizeUnits = sessionInfo.Units?.SizeBytes ?? 0,
            MemoryUnits = sessionInfo.Units?.MemoryBytes ?? 0,
            DownloadQueueEnabled = sessionInfo.DownloadQueueEnabled,
            DownloadQueueSize = sessionInfo.DownloadQueueSize,
            SeedQueueEnabled = sessionInfo.SeedQueueEnabled,
            SeedQueueSize = sessionInfo.SeedQueueSize,
            PortForwardingEnabled = sessionInfo.PortForwardingEnabled,
            PeerPort = sessionInfo.PeerPort
        };

        return model;
    }

    public Task SetSessionSettingsAsync(SessionSettingsModel model, CancellationToken cancellationToken = default)
    {
        SessionSettings settings = new();

        if (model.CacheSize.HasValue)
            settings.CacheSizeMb = model.CacheSize.Value;
        if (model.CompletePath != null)
            settings.DownloadDirectory = model.CompletePath;
        if (model.InCompletePathEnabled.HasValue)
            settings.IncompleteDirectoryEnabled = model.InCompletePathEnabled.Value;
        if (model.InCompletePath != null)
            settings.IncompleteDirectory = model.InCompletePath;
        if (model.UsePex.HasValue)
            settings.PexEnabled = model.UsePex.Value;
        if (model.UseLpd.HasValue)
            settings.LpdEnabled = model.UseLpd.Value;
        if (model.UseDht.HasValue)
            settings.DhtEnabled = model.UseDht.Value;
        if (model.UseUdp.HasValue)
            settings.UtpEnabled = model.UseUdp.Value;
        if (model.EncryptionMode.HasValue)
            settings.Encryption = model.EncryptionMode.Value.ToString().ToLower();
        if (model.DownloadQueueEnabled.HasValue)
            settings.DownloadQueueEnabled = model.DownloadQueueEnabled.Value;
        if (model.DownloadQueueSize.HasValue)
            settings.DownloadQueueSize = model.DownloadQueueSize.Value;
        if (model.SeedQueueEnabled.HasValue)
            settings.SeedQueueEnabled = model.SeedQueueEnabled.Value;
        if (model.SeedQueueSize.HasValue)
            settings.SeedQueueSize = model.SeedQueueSize.Value;
        if (model.PortForwardingEnabled.HasValue)
            settings.PortForwardingEnabled = model.PortForwardingEnabled.Value;
        if (model.PeerPort.HasValue)
            settings.PeerPort = model.PeerPort.Value;

        return _client.SetSessionSettingsAsync(settings, cancellationToken);
    }

    public async Task<StatisticModel> GetStatisticModelAsync(CancellationToken cancellationToken = default)
    {
        Statistic statistic = await _client.GetSessionStatisticAsync(cancellationToken);

        CommonStatisticModel currentStat = new();
        CommonStatisticModel cumulativeStat = new();

        currentStat.DownloadedBytes = statistic.CurrentStats.DownloadedBytes;
        currentStat.UploadedBytes = statistic.CurrentStats.UploadedBytes;
        currentStat.SecondsActive = statistic.CurrentStats.SecondsActive;
        currentStat.FilesAdded = statistic.CurrentStats.FilesAdded;
        currentStat.SessionCount = statistic.CurrentStats.SessionCount;

        cumulativeStat.DownloadedBytes = statistic.CumulativeStats.DownloadedBytes;
        cumulativeStat.UploadedBytes = statistic.CumulativeStats.UploadedBytes;
        cumulativeStat.SecondsActive = statistic.CumulativeStats.SecondsActive;
        cumulativeStat.FilesAdded = statistic.CumulativeStats.FilesAdded;
        cumulativeStat.SessionCount = statistic.CumulativeStats.SessionCount;

        return new StatisticModel
        {
            DownloadSpeed = statistic.DownloadSpeed,
            UploadSpeed = statistic.UploadSpeed,
            ActiveTorrentCount = statistic.ActiveTorrentCount,
            PausedTorrentCount = statistic.PausedTorrentCount,
            TorrentCount = statistic.TorrentCount,
            CurrentStats = currentStat,
            CumulativeStats = cumulativeStat
        };
    }

    public async Task<TorrentModel[]> GetTorrentModelAsync(TorrentField[] fields, int[]? ids = null, CancellationToken cancellationToken = default)
    {
        HashSet<TorrentField> fieldSet = new(fields) { TorrentField.Id };
        string[] rpcFields = fieldSet.Select(f => FieldMap[f]).ToArray();
        object[]? rpcIds = ids?.Select<int, object>(id => id).ToArray();

        TransmissionTorrents torrents = await _client.TorrentGetAsync(rpcFields, rpcIds, cancellationToken);
        TorrentInfo[] torrentInfos = torrents.Torrents;

        TorrentModel[] models = new TorrentModel[torrentInfos.Length];

        for (int i = 0; i < models.Length; i++)
        {
            TorrentInfo info = torrentInfos[i];

            models[i] = new TorrentModel
            {
                Id = info.Id,
                QueuePosition = info.QueuePosition,
                Name = info.Name,
                TotalSize = info.TotalSize,
                Progress = info.PercentDone,
                VerifyingProgress = info.RecheckProgress,
                DownloadRate = info.RateDownload,
                UploadRate = info.RateUpload,
                UploadRatio = info.UploadRatio,
                LeftUntilDone = info.LeftUntilDone,
                SecondsDownloading = info.SecondsDownloading,
                Status = (TorrentModel.State)info.Status,
                DownloadDir = info.DownloadDir,
                DateAdded = info.AddedDate,
                SequentialDownload =  info.SequentialDownload
            };

            if (fieldSet.Contains(TorrentField.Files) && info.Files != null)
            {
                TransmissionTorrentFiles[] files = info.Files;
                TransmissionTorrentFileStats[] fileStats = info.FileStats;
                TorrentFileModel[] fileModels = new TorrentFileModel[files.Length];

                for (int j = 0; j < fileModels.Length; j++)
                {
                    fileModels[j] = new TorrentFileModel
                    {
                        Name = files[j].Name,
                        Length = files[j].Length,
                        Index = j,
                        BytesCompleted = files[j].BytesCompleted,
                        Wanted = fileStats != null && j < fileStats.Length && fileStats[j].Wanted,
                        Priority = fileStats != null && j < fileStats.Length ? (FilePriority)fileStats[j].Priority : FilePriority.Normal,
                        BeginPiece = files[j].BeginPiece,
                        PieceCount = files[j].EndPiece - files[j].BeginPiece
                    };
                }

                models[i].Files = fileModels;
            }

            if (fieldSet.Contains(TorrentField.Availability) && info.Availability != null)
                models[i].Availability = info.Availability;

            if (fieldSet.Contains(TorrentField.Pieces) && !string.IsNullOrEmpty(info.Pieces))
                models[i].Pieces = Convert.FromBase64String(info.Pieces);
        }

        return models;
    }

    public Task SetTorrentSettingsAsync(TorrentSettingsModel model, CancellationToken cancellationToken = default)
    {
        TorrentSettings settings = new()
        {
            Ids = [model.Id]
        };

        if (model.SequentialDownload.HasValue)
            settings.SequentialDownload = model.SequentialDownload.Value;

        if (model.FileSettings != null)
        {
            int[] wanted = model.FileSettings.Where(f => f.Wanted).Select(f => f.Id).ToArray();
            int[] unwanted = model.FileSettings.Where(f => !f.Wanted).Select(f => f.Id).ToArray();

            if (wanted.Length > 0)
                settings.FilesWanted = wanted;

            if (unwanted.Length > 0)
                settings.FilesUnwanted = unwanted;

            int[] priorityHigh = model.FileSettings.Where(f => f.Priority == FilePriority.High).Select(f => f.Id).ToArray();
            int[] priorityNormal = model.FileSettings.Where(f => f.Priority == FilePriority.Normal).Select(f => f.Id).ToArray();
            int[] priorityLow = model.FileSettings.Where(f => f.Priority == FilePriority.Low).Select(f => f.Id).ToArray();

            if (priorityHigh.Length > 0)
                settings.PriorityHigh = priorityHigh;

            if (priorityNormal.Length > 0)
                settings.PriorityNormal = priorityNormal;

            if (priorityLow.Length > 0)
                settings.PriorityLow = priorityLow;
        }

        return _client.TorrentSetAsync(settings, cancellationToken);
    }

    public Task AddTorrentAsync(string metaInfo, AddTorrentOptions? options = null, CancellationToken cancellationToken = default)
    {
        NewTorrent torrent = new NewTorrent { Metainfo = metaInfo };

        if (options != null)
        {
            torrent.DownloadDirectory = options.DownloadDirectory;
            torrent.Paused = options.Paused;
            torrent.SequentialDownload = options.SequentialDownload;
        }

        return _client.TorrentAddAsync(torrent, cancellationToken);
    }

    public Task StartNowTorrentAsync(int id, CancellationToken cancellationToken = default)
    {
        return _client.TorrentStartNowAsync([id], cancellationToken);
    }

    public Task StartTorrentAsync(int id, CancellationToken cancellationToken = default)
    {
        return _client.TorrentStartAsync([id], cancellationToken);
    }

    public Task StopTorrentAsync(int id, CancellationToken cancellationToken = default)
    {
        return _client.TorrentStopAsync([id], cancellationToken);
    }

    public Task DeleteTorrentAsync(int id, bool deleteFiles, CancellationToken cancellationToken = default)
    {
        return _client.TorrentRemoveAsync([id], deleteFiles, cancellationToken);
    }

    public Task VerifyTorrentAsync(int id, CancellationToken cancellationToken = default)
    {
        return _client.TorrentVerifyAsync([id], cancellationToken);
    }

    public Task DeleteAllAsync(int[] ids, CancellationToken cancellationToken = default)
    {
        return _client.TorrentRemoveAsync(ids.Select<int, object>(x => x).ToArray(), false, cancellationToken);
    }

    public Task StartAllAsync(int[] ids, CancellationToken cancellationToken = default)
    {
        return _client.TorrentStartAsync(ids.Select<int, object>(x => x).ToArray(), cancellationToken);
    }

    public Task StopAllAsync(int[] ids, CancellationToken cancellationToken = default)
    {
        return _client.TorrentStopAsync(ids.Select<int, object>(x => x).ToArray(), cancellationToken);
    }

    public async Task<bool> PortTestAsync(CancellationToken cancellationToken = default)
    {
        var result = await _client.PortTestAsync(cancellationToken);
        return result.PortIsOpen;
    }
}
