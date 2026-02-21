namespace TransmissionNet.TorrentProviders;

public class AddTorrentOptions
{
    public required string DownloadDirectory { get; init; }
    public bool Paused { get; init; }
    public bool SequentialDownload { get; init; }
}
