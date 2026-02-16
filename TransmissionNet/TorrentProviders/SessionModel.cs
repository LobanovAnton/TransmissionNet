namespace TransmissionNet.TorrentProviders;

public enum Encryption
{
    Required,
    Preferred,
    Allowed
}

public class SessionModel
{
    public string Version { get; set; } = string.Empty;
    
    public int SpeedUnits { get; set; }
    
    public int SizeUnits { get; set; }
    
    public int MemoryUnits { get; set; }
    
    public int CacheSize { get; set; }
    
    public string CompletePath { get; set; } = string.Empty;
    
    public bool InCompletePathEnabled { get; set; }
    
    public string InCompletePath { get; set; } = string.Empty;
    
    public bool DownloadQueueEnabled { get; set; }
    
    public int DownloadQueueSize { get; set; }
    
    public bool SeedQueueEnabled { get; set; }
    
    public int SeedQueueSize { get; set; }
    
    public bool PortForwardingEnabled { get; set; }
    
    public int PeerPort { get; set; }
    
    public Encryption EncryptionMode { get; set; }
    
    public bool UsePex { get; set; }
    
    public bool UseLpd { get; set; }
    
    public bool UseDht { get; set; }
    
    public bool UseUdp { get; set; }
}