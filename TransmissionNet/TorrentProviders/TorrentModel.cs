namespace TransmissionNet.TorrentProviders;

public class TorrentModel
{
    public enum State
    {
        Stopped = 0,
        WaitVerify = 1,
        Verifying = 2,
        WaitDownload = 3,
        Downloading = 4,
        WaitingSeed = 5,
        Seeding = 6
    }

    public required int Id { get; init; }
    public required string Name { get; init; }
    public required long TotalSize { get; init; }
    public required long DateAdded { get; init; }
    public required string DownloadDir { get; init; }
    public int QueuePosition { get; init; }
    public double Progress { get; init; }
    public double VerifyingProgress { get; init; }
    public int DownloadRate { get; init; }
    public int UploadRate { get; init; }
    public double UploadRatio { get; init; }
    public long LeftUntilDone { get; init; }
    public int SecondsDownloading { get; init; }
    public State Status { get; init; }

    public int[] Availability { get; set; } = [];
    public TorrentFileModel[] Files { get; set; } = [];
    public byte[] Pieces { get; set; } = [];
    public bool SequentialDownload { get; set; }
}
