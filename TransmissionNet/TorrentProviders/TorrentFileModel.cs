namespace TransmissionNet.TorrentProviders;

public enum FilePriority
{
    Low = -1,
    Normal = 0,
    High = 1
}

public class TorrentFileModel
{
    public required string Name { get; init; }
    public required long Length { get; init; }
    public required int Index { get; init; }
    public long BytesCompleted { get; init; }
    public bool Wanted { get; init; }
    public FilePriority Priority { get; init; }
    public int BeginPiece { get; init; }
    public int PieceCount { get; init; }
}
