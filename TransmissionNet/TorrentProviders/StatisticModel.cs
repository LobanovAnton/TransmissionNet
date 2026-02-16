namespace TransmissionNet.TorrentProviders;

public class CommonStatisticModel: BindableObject
{
    public double UploadedBytes { get; set; }

    public double DownloadedBytes { get; set; }

    public int FilesAdded { get; set; }

    public int SessionCount { get; set; }

    public int SecondsActive { get; set; }
}

public class StatisticModel
{
    public static readonly StatisticModel Empty = new()
    {
        CurrentStats = new(),
        CumulativeStats = new()
    };

    public int ActiveTorrentCount { get; set; }

    public int DownloadSpeed { get; set; }

    public int PausedTorrentCount { get; set; }
    
    public int TorrentCount { get; set; }

    public int UploadSpeed { get; set; }

    public required CommonStatisticModel CumulativeStats { get; init; }
    
    public required CommonStatisticModel CurrentStats { get; init; }
}
