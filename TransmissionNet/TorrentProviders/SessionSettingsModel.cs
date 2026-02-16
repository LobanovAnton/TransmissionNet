namespace TransmissionNet.TorrentProviders;

public class SessionSettingsModel
{
    public int? CacheSize { get; set; }

    public string? CompletePath { get; set; }

    public bool? InCompletePathEnabled { get; set; }

    public string? InCompletePath { get; set; }

    public bool? DownloadQueueEnabled { get; set; }

    public int? DownloadQueueSize { get; set; }

    public bool? SeedQueueEnabled { get; set; }

    public int? SeedQueueSize { get; set; }

    public bool? PortForwardingEnabled { get; set; }

    public int? PeerPort { get; set; }

    public Encryption? EncryptionMode { get; set; }

    public bool? UsePex { get; set; }

    public bool? UseLpd { get; set; }

    public bool? UseDht { get; set; }

    public bool? UseUdp { get; set; }
}
