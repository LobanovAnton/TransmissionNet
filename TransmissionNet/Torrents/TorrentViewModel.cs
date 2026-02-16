using TransmissionNet.TorrentProviders;
using TransmissionNet.Utils;

namespace TransmissionNet.Torrents;

public class TorrentViewModel : BindableObject
{
    private readonly MedianFilter<int> _filter;
    
    private int _queuePosition;
    private double _progress;
    private double _verifyingProgress;
    private int _downloadRate;
    private int _uploadRate;
    private double _uploadRatio;
    private long _leftUntilDone;
    private int _secondsDownloading;
    private long _downloaded;
    private long _uploaded;
    private int _secondsRemaining;
    private TorrentModel.State _status;

    public TorrentViewModel(TorrentModel model)
    {
        _filter = new MedianFilter<int>(11, 80);
        
        Id = model.Id;
        Name = model.Name;
        TotalSize = model.TotalSize;
        DateAdded = model.DateAdded;
        DownloadDir = model.DownloadDir;
        _queuePosition = model.QueuePosition;
        _progress = model.Progress;
        _verifyingProgress = model.VerifyingProgress;
        _downloadRate = model.DownloadRate;
        _uploadRate = model.UploadRate;
        _uploadRatio = model.UploadRatio;
        _leftUntilDone = model.LeftUntilDone;
        _secondsDownloading = model.SecondsDownloading;
        _status = model.Status;
        _uploaded = (long)(TotalSize * _uploadRatio);
        _downloaded = TotalSize - _leftUntilDone;
        _secondsRemaining = CalculateSecondsRemaining(_downloadRate, _leftUntilDone);
    }

    public int Id { get; }

    public string Name { get; }

    public long TotalSize { get; }

    public long DateAdded { get; }

    public string DownloadDir { get; }

    public int QueuePosition
    {
        get => _queuePosition;
        private set
        {
            if (value == _queuePosition) return;
            _queuePosition = value;
            OnPropertyChanged();
        }
    }

    public double Progress
    {
        get => _progress;
        private set
        {
            if (value.Equals(_progress)) return;
            _progress = value;
            OnPropertyChanged();
        }
    }

    public double VerifyingProgress
    {
        get => _verifyingProgress;
        private set
        {
            if (value.Equals(_verifyingProgress)) return;
            _verifyingProgress = value;
            OnPropertyChanged();
        }
    }

    public int DownloadRate
    {
        get => _downloadRate;
        private set
        {
            if (value == _downloadRate) return;
            _downloadRate = value;
            OnPropertyChanged();
        }
    }

    public int UploadRate
    {
        get => _uploadRate;
        private set
        {
            if (value == _uploadRate) return;
            _uploadRate = value;
            OnPropertyChanged();
        }
    }

    public double UploadRatio
    {
        get => _uploadRatio;
        private set
        {
            if (value.Equals(_uploadRatio)) return;
            _uploadRatio = value;
            _uploaded = (long)(TotalSize * _uploadRatio);
            OnPropertyChanged();
            OnPropertyChanged(nameof(Uploaded));
        }
    }

    public long LeftUntilDone
    {
        get => _leftUntilDone;
        private set
        {
            if (value == _leftUntilDone) return;
            _leftUntilDone = value;
            _downloaded = TotalSize - _leftUntilDone;
            _secondsRemaining = CalculateSecondsRemaining(_downloadRate, _leftUntilDone);
            OnPropertyChanged();
            OnPropertyChanged(nameof(Downloaded));
            OnPropertyChanged(nameof(SecondsRemaining));
        }
    }

    public int SecondsDownloading
    {
        get => _secondsDownloading;
        private set
        {
            if (value == _secondsDownloading) return;
            _secondsDownloading = value;
            _secondsRemaining = CalculateSecondsRemaining(_downloadRate, _leftUntilDone);
            OnPropertyChanged();
            OnPropertyChanged(nameof(SecondsRemaining));
        }
    }

    public TorrentModel.State Status
    {
        get => _status;
        private set
        {
            if (value == _status) return;
            _status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsVerifying));
        }
    }

    public bool IsVerifying => _status == TorrentModel.State.Verifying || _status == TorrentModel.State.WaitVerify;

    public long Uploaded => _uploaded;

    public long Downloaded => _downloaded;

    public int SecondsRemaining => _secondsRemaining;

    public void Update(TorrentModel model)
    {
        Progress = model.Progress;
        VerifyingProgress = model.VerifyingProgress;
        DownloadRate = model.DownloadRate;
        UploadRate = model.UploadRate;
        UploadRatio = model.UploadRatio;
        LeftUntilDone = model.LeftUntilDone;
        SecondsDownloading = model.SecondsDownloading;
        Status = model.Status;
    }

    private int CalculateSecondsRemaining(int downloadRate, long leftUntilDone)
    {
        int secondsRemaining = int.MaxValue;
        
        _filter.AddValue(downloadRate);
        downloadRate = _filter.GetValue();

        if (downloadRate != 0)
            secondsRemaining = (int)(leftUntilDone / downloadRate);

        return secondsRemaining;
    }
}
