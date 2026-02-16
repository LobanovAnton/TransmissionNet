using System.Collections.ObjectModel;
using TransmissionNet.Settings;
using TransmissionNet.Shell;
using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Torrents;

public class FilePageViewModel : UpdatableShellPageViewModel
{
    private static readonly TorrentField[] FileFields =
    [
        TorrentField.Id, TorrentField.Name, TorrentField.Availability, TorrentField.Files, TorrentField.FileStats,
        TorrentField.Pieces, TorrentField.PieceCount, TorrentField.SequentialDownload
    ];

    private TorrentViewModel? _torrent;
    private bool _sequentialDownload;
    private CancellationTokenSource? _torrentCts;

    public ObservableCollection<FileViewModel> Items { get; } = [];

    public int[] Availability
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    } = [];

    public byte[] Pieces
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    } = [];

    public bool SequentialDownload
    {
        get => _sequentialDownload;
        set
        {
            if (value == _sequentialDownload) return;
            _sequentialDownload = value;
            OnPropertyChanged();
            SaveSequentialDownloadAsync();
        }
    }

    public TorrentViewModel? Torrent
    {
        get => _torrent;
        set
        {
            if (_torrent?.Id == value?.Id) return;
            _torrentCts?.Cancel();
            _torrentCts?.Dispose();
            _torrentCts = new CancellationTokenSource();
            Items.Clear();
            _torrent = value;
            OnPropertyChanged();
        }
    }

    private async Task LoadFilesAsync(CancellationToken cancellationToken)
    {
        if (Provider == null || _torrent == null)
            return;

        TorrentModel[] models = await Provider.GetTorrentModelAsync(FileFields, [_torrent.Id], cancellationToken);

        if (models.Length == 0 || models[0].Files.Length == 0)
        {
            Torrent = null;
            return;
        }

        TorrentModel model = models[0];
        TorrentFileModel[] files = model.Files;
        Availability = model.Availability;
        Pieces = model.Pieces;

        if (_sequentialDownload != model.SequentialDownload)
        {
            _sequentialDownload = model.SequentialDownload;
            OnPropertyChanged(nameof(SequentialDownload));
        }

        if (Items.Count == 0)
        {
            foreach (TorrentFileModel file in files)
                Items.Add(new FileViewModel(Provider, _torrent.Id, file));
        }
        else
        {
            for (int i = 0; i < files.Length; i++)
                Items[i].Update(files[i]);
        }
    }

    private async void SaveSequentialDownloadAsync()
    {
        try
        {
            if (Provider == null || _torrent == null)
                return;

            TorrentSettingsModel settings = new()
            {
                Id = _torrent.Id,
                SequentialDownload = _sequentialDownload
            };

            await Provider.SetTorrentLocationAsync(_torrent.Id, (string)SettingEntries.CompletePath.Value);
            await Provider.SetTorrentSettingsAsync(settings);
        }
        catch (Exception)
        {
            // ignored
        }
    }

    protected override async Task OnUpdateAsync(CancellationToken cancellationToken)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, _torrentCts?.Token ?? CancellationToken.None);
        await LoadFilesAsync(linked.Token);
    }
}
