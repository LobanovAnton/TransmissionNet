namespace TransmissionNet.TorrentProviders;

public class TorrentFileSettingsModel
{
    public required int Id { get; init; }

    public required bool Wanted { get; init; }

    public FilePriority Priority { get; init; }
}
