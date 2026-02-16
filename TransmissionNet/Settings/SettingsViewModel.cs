using System.Collections.ObjectModel;
using System.Globalization;
using TransmissionNet.Shell;
using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Settings;

public enum ValueType
{
    Uri,
    Login,
    Password,
    Path,
    AsciiDigit,
    CheckBox,
    Radio3Button
}

public static class SettingViewModels
{
    public static readonly SettingViewModel Url = new(){Entry = SettingEntries.Url, Title = "Url", ValueType = ValueType.Uri, PlaceHolder = "Please enter server URL"};
    public static readonly SettingViewModel Port = new(){Entry = SettingEntries.Port, Title = "Port", ValueType = ValueType.AsciiDigit, PlaceHolder = "Please enter server port"};
    public static readonly SettingViewModel RpcPath = new(){Entry = SettingEntries.RpcPath, Title = "Rpc path", ValueType = ValueType.Path, PlaceHolder = "Please enter server RPC path"};
    public static readonly SettingViewModel Login = new(){Entry = SettingEntries.Login, Title = "Login", ValueType = ValueType.Login, PlaceHolder = "Please enter your username"};
    public static readonly SettingViewModel Password = new(){Entry = SettingEntries.Password, Title = "Password", ValueType = ValueType.Password, PlaceHolder = "Please enter your password"};
    public static readonly SettingViewModel UpdateInterval = new(){Entry = SettingEntries.UpdateInterval, Title = "Update interval", ValueType = ValueType.AsciiDigit, PlaceHolder = "Please enter update interval"};
    public static readonly SettingViewModel DeleteTorrentFile = new(){Entry = SettingEntries.DeleteTorrentFile, Title = "Delete torrent file", ValueType = ValueType.CheckBox};
    
    public static readonly SettingViewModel CacheSize = new(){Entry = SettingEntries.CacheSize, Title = "Cache size", ValueType = ValueType.AsciiDigit};
    public static readonly SettingViewModel CompletePath = new(){Entry = SettingEntries.CompletePath, Title = "Complete directory", ValueType = ValueType.Path};
    public static readonly SettingViewModel InCompletePathEnabled = new(){Entry = SettingEntries.InCompletePathEnabled, Title = "Incomplete directory enabled", ValueType = ValueType.CheckBox};
    public static readonly SettingViewModel InCompletePath = new(){Entry = SettingEntries.InCompletePath, Title = "Incomplete directory", ValueType = ValueType.Path};
    public static readonly SettingViewModel UsePex = new(){Entry = SettingEntries.UseUdp, Title = "Peer exchange", ValueType = ValueType.CheckBox};
    public static readonly SettingViewModel UseLpd = new(){Entry = SettingEntries.UseLpd, Title = "Local peer discovery", ValueType = ValueType.CheckBox};
    public static readonly SettingViewModel UseDht = new(){Entry = SettingEntries.UseDht, Title = "Dht", ValueType = ValueType.CheckBox};
    public static readonly SettingViewModel UseUdp = new(){Entry = SettingEntries.UseUdp, Title = "Udp", ValueType = ValueType.CheckBox};
    public static readonly SettingViewModel PeerPort = new(){Entry = SettingEntries.PeerPort, Title = "Peer port", ValueType = ValueType.AsciiDigit};
    public static readonly SettingViewModel PortForwardingEnabled = new(){Entry = SettingEntries.PortForwardingEnabled, Title = "Port forwarding enabled", ValueType = ValueType.CheckBox};
    public static readonly SettingViewModel DownloadQueueEnabled = new(){Entry = SettingEntries.DownloadQueueEnabled, Title = "Download queue enabled", ValueType = ValueType.CheckBox};
    public static readonly SettingViewModel DownloadQueueSize = new(){Entry = SettingEntries.DownloadQueueSize, Title = "Download queue size", ValueType = ValueType.AsciiDigit};
    public static readonly SettingViewModel SeedQueueEnabled = new(){Entry = SettingEntries.SeedQueueEnabled, Title = "Seed queue enabled", ValueType = ValueType.CheckBox};
    public static readonly SettingViewModel SeedQueueSize = new(){Entry = SettingEntries.SeedQueueSize, Title = "Seed queue size", ValueType = ValueType.AsciiDigit};
    public static readonly Radio3SettingViewModel EncryptionMode = 
        new(){Entry = SettingEntries.EncryptionMode, 
            Title = "Encryption", 
            ValueType = ValueType.Radio3Button,
            Group = "encryption",
            Value1 = Encryption.Required,
            Title1 = nameof(Encryption.Required),
            Value2 = Encryption.Preferred,
            Title2 = nameof(Encryption.Preferred),
            Value3 = Encryption.Allowed,
            Title3 = nameof(Encryption.Allowed),
        };
    
    public static readonly SettingViewModel[] ApplicationSettings = [Url, Port, RpcPath, Login, Password, UpdateInterval, DeleteTorrentFile];
    public static readonly SettingViewModel[] TorrentSettings = [CompletePath, InCompletePathEnabled, InCompletePath, CacheSize,
                                                                 DownloadQueueEnabled, DownloadQueueSize,
                                                                 SeedQueueEnabled, SeedQueueSize,
                                                                 UsePex, UseLpd, UseDht, UseUdp,
                                                                 EncryptionMode, PortForwardingEnabled, PeerPort];
}

public class Radio3SettingViewModel : SettingViewModel
{
    public required string Group { get; init; }

    public required string Title1 { get; init; }
    
    public required string Title2 { get; init; }
    
    public required string Title3 { get; init; }
    
    public required object Value1 { get; init; }
    
    public required object Value2 { get; init; }
    
    public required object Value3 { get; init; }

    public bool IsChecked1
    {
        get => Value1.Equals(Entry.Value);
        set
        {
            if (value)
                Entry.Value = Value1;
        }
    }

    public bool IsChecked2
    {
        get => Value2.Equals(Entry.Value);
        set
        {
            if (value)
                Entry.Value = Value2;
        }
    }

    public bool IsChecked3
    {
        get => Value3.Equals(Entry.Value);
        set
        {
            if (value)
                Entry.Value = Value3;
        }
    }
}

public class SettingViewModel
{
    public required SettingEntry Entry { get; init; }

    public string PlaceHolder { get; set; } = string.Empty;
    
    public required ValueType ValueType {get; init;}

    public required string Title { get; init; }
    
    public event Action? ValueChanged;

    public string Value
    {
        get => Entry.Value.ToString() ?? string.Empty;
        set
        {
            switch (Entry.DefaultValue.GetType())
            {
                case { } type when type == typeof(string):
                    Entry.Value = value;
                    break;
                case { } type when type == typeof(int):
                    int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue);
                    Entry.Value = intValue;
                    break;
                case { } type when type == typeof(bool):
                    bool.TryParse(value, out bool boolValue);
                    Entry.Value = boolValue;
                    break;
            }
            ValueChanged?.Invoke();
        }
    }
}

public class ValueTypeSelector : DataTemplateSelector
{
    public DataTemplate? UrlTemplate { get; set; }
    
    public DataTemplate? NumericTemplate { get; set; }
    
    public DataTemplate? LoginTemplate { get; set; }
    
    public DataTemplate? PasswordTemplate { get; set; }
    
    public DataTemplate? PathTemplate { get; set; }
    
    public DataTemplate? CheckBoxTemplate { get; set; }
    
    public DataTemplate? Radio3ButtonTemplate { get; set; }
    
    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        ValueType valueType = ((SettingViewModel)item).ValueType;
        return valueType switch
        {
            ValueType.Uri => UrlTemplate,
            ValueType.Login => LoginTemplate,
            ValueType.Password => PasswordTemplate,
            ValueType.AsciiDigit => NumericTemplate,
            ValueType.Path => PathTemplate,
            ValueType.CheckBox => CheckBoxTemplate,
            ValueType.Radio3Button => Radio3ButtonTemplate,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public abstract class SettingsViewModel : ProviderShellPageViewModel
{
    private string _version = string.Empty;
    
    public ObservableCollection<SettingViewModel> SettingsCollection { get;} = [];

    public string Version
    {
        get => _version;
        set
        {
            if (_version == value) return;
            _version = value;
            OnPropertyChanged();
        }
    }
}
