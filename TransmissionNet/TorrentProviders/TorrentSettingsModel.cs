namespace TransmissionNet.TorrentProviders;

public class TorrentSettingsModel
{
    public required int Id { get; init; }

    public bool? SequentialDownload { get; init; }

    public TorrentFileSettingsModel[]? FileSettings { get; init; }
}
