using TransmissionNet.TorrentProviders;
using TransmissionNet.Settings.SettingsProviders;

namespace TransmissionNet.Settings;

[Flags]
public enum SettingsType
{
    Application = 0x1,
    Server = 0x2,
    
    All = Application | Server
}

public static class SettingEntries
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private static readonly PreferencesSettingsProvider PreferencesSettingsProvider = new();
    private static readonly TorrentSettingsProvider TorrentSettingsProvider = new();
    
    public static readonly SettingEntry Url = new(){Key = nameof(Url), DefaultValue = "http://"};
    public static readonly SettingEntry Port = new(){Key = nameof(Port), DefaultValue = 9091};
    public static readonly SettingEntry RpcPath = new(){Key = nameof(RpcPath), DefaultValue = "transmission/rpc"};
    public static readonly SettingEntry Login = new(){Key = nameof(Login), DefaultValue = ""};
    public static readonly SettingEntry Password = new(){Key = nameof(Password), DefaultValue = ""};
    public static readonly SettingEntry UpdateInterval = new(){Key = nameof(UpdateInterval), DefaultValue = 5};
    public static readonly SettingEntry DeleteTorrentFile = new(){Key = nameof(DeleteTorrentFile), DefaultValue = false};
    
    public static readonly SettingEntry CacheSize = new(){Key = nameof(SessionModel.CacheSize), DefaultValue = 16};
    public static readonly SettingEntry CompletePath = new(){Key = nameof(SessionModel.CompletePath), DefaultValue = ""};
    public static readonly SettingEntry InCompletePathEnabled = new(){Key = nameof(SessionModel.InCompletePathEnabled), DefaultValue = true};
    public static readonly SettingEntry InCompletePath = new(){Key = nameof(SessionModel.InCompletePath), DefaultValue = ""};
    public static readonly SettingEntry UsePex = new(){Key = nameof(SessionModel.UsePex), DefaultValue = true};
    public static readonly SettingEntry UseLpd = new(){Key = nameof(SessionModel.UseLpd), DefaultValue = true};
    public static readonly SettingEntry UseDht = new(){Key = nameof(SessionModel.UseDht), DefaultValue = true};
    public static readonly SettingEntry UseUdp = new(){Key = nameof(SessionModel.UseUdp), DefaultValue = true};
    public static readonly SettingEntry EncryptionMode = new(){Key = nameof(SessionModel.EncryptionMode), DefaultValue = Encryption.Allowed};
    public static readonly SettingEntry DownloadQueueEnabled = new(){Key = nameof(SessionModel.DownloadQueueEnabled), DefaultValue = true};
    public static readonly SettingEntry DownloadQueueSize = new(){Key = nameof(SessionModel.DownloadQueueSize), DefaultValue = 5};
    public static readonly SettingEntry SeedQueueEnabled = new(){Key = nameof(SessionModel.SeedQueueEnabled), DefaultValue = false};
    public static readonly SettingEntry SeedQueueSize = new(){Key = nameof(SessionModel.SeedQueueSize), DefaultValue = 10};
    public static readonly SettingEntry PeerPort = new(){Key = nameof(SessionModel.PeerPort), DefaultValue = 51413};
    public static readonly SettingEntry PortForwardingEnabled = new(){Key = nameof(SessionModel.PortForwardingEnabled), DefaultValue = false};
    
    private static readonly SettingEntry[] ApplicationSettings = [Url, Port, RpcPath, Login, Password, UpdateInterval, DeleteTorrentFile];
    private static readonly SettingEntry[] TorrentSettings = [CacheSize, CompletePath, InCompletePathEnabled, InCompletePath, 
                                                              UsePex, UseLpd, UseDht, UseUdp,
                                                              EncryptionMode, DownloadQueueEnabled, DownloadQueueSize,
                                                              SeedQueueEnabled, SeedQueueSize, PeerPort, PortForwardingEnabled];

    public static async Task LoadEntries(SettingsType settingsType)
    {
        await Semaphore.WaitAsync();

        if ((settingsType & SettingsType.Application) != 0)
            await PreferencesSettingsProvider.LoadValuesAsync(ApplicationSettings);
        if ((settingsType & SettingsType.Server) != 0)
        {
            string fullUrl = $"{Url.Value}:{Port.Value}/{RpcPath.Value}";
            TorrentSettingsProvider.Initialize(fullUrl, (string)Login.Value, (string)Password.Value);
            await TorrentSettingsProvider.LoadValuesAsync(TorrentSettings);
        }

        Semaphore.Release();
    }

    public static async Task SaveEntries(SettingsType settingsType)
    {
        await Semaphore.WaitAsync();

        if ((settingsType & SettingsType.Application) != 0)
        {
            await PreferencesSettingsProvider.SaveValuesAsync(ApplicationSettings);
            Console.WriteLine(ApplicationSettings[6].Value);
        }
            
        if ((settingsType & SettingsType.Server) != 0)
            await TorrentSettingsProvider.SaveValuesAsync(TorrentSettings);
        
        Semaphore.Release();
    }
}

public class SettingEntry
{
    public bool IsValid { get; set; }
    
    public required string Key {get; init;}

    public required object DefaultValue {get; init;}

    public object Value { get; set; } = new();
}
