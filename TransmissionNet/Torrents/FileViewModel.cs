using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Torrents;

public class FileViewModel : BindableObject
{
    private readonly ITorrentProvider _provider;
    private readonly int _torrentId;
    private readonly int _fileIndex;
    private long _bytesCompleted;
    private bool _wanted;
    private FilePriority _priority;
    private bool _isExpanded;

    public FileViewModel(ITorrentProvider provider, int torrentId, TorrentFileModel file)
    {
        _provider = provider;
        _torrentId = torrentId;
        _fileIndex = file.Index;
        Name = file.Name;
        Length = file.Length;
        BeginPiece = file.BeginPiece;
        PieceCount = file.PieceCount;
        _bytesCompleted = file.BytesCompleted;
        _wanted = file.Wanted;
        _priority = file.Priority;
    }

    public string Name { get; }

    public long Length { get; }

    public int BeginPiece { get; }

    public int PieceCount { get; }

    public long BytesCompleted
    {
        get => _bytesCompleted;
        private set
        {
            if (value == _bytesCompleted) return;
            bool isCompleted = IsCompleted;
            _bytesCompleted = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Progress));
            if (isCompleted != IsCompleted)
                OnPropertyChanged(nameof(IsCompleted));
        }
    }

    public bool Wanted
    {
        get => _wanted;
        set
        {
            if (value == _wanted) return;
            _wanted = value;
            OnPropertyChanged();
            SaveFileSettingsAsync();
        }
    }

    public FilePriority Priority
    {
        get => _priority;
        set
        {
            if (value == _priority) return;
            _priority = value;
            OnPropertyChanged();
            SaveFileSettingsAsync();
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (value == _isExpanded) return;
            _isExpanded = value;
            OnPropertyChanged();
        }
    }

    public bool IsCompleted => _bytesCompleted == Length;

    public double Progress => Length > 0 ? (double)_bytesCompleted / Length : 0;

    public void Update(TorrentFileModel file)
    {
        BytesCompleted = file.BytesCompleted;

        if (_wanted != file.Wanted)
        {
            _wanted = file.Wanted;
            OnPropertyChanged(nameof(Wanted));
        }

        if (_priority != file.Priority)
        {
            _priority = file.Priority;
            OnPropertyChanged(nameof(Priority));
        }
    }

    private async void SaveFileSettingsAsync()
    {
        try
        {
            TorrentFileSettingsModel fileSettings = new()
            {
                Id = _fileIndex,
                Wanted = _wanted,
                Priority = _priority
            };

            TorrentSettingsModel torrentSettings = new()
            {
                Id = _torrentId,
                FileSettings = [fileSettings]
            };

            await _provider.SetTorrentSettingsAsync(torrentSettings);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
