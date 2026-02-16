using TransmissionNet.Settings;
using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Shell;

public abstract class ShellPageViewModel: BindableObject
{
    private bool _isRunning;
    private bool _isInitialized;
    
    public bool IsRunning
    {
        get => _isRunning;
        protected set
        {
            if (value == _isRunning) return;
            _isRunning = value;
            OnPropertyChanged();
        }
    }
    
    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        _isInitialized = true;
        
        IsRunning = true;

        try
        {
            await OnInitializeAsync();
        }
        catch (Exception)
        {
            // ignored
        }
        
        IsRunning = false;
    }

    public async Task DeinitializeAsync()
    {
        _isInitialized = false;
        
        IsRunning = true;

        try
        {
            await OnDeinitializeAsync();
        }
        catch (Exception)
        {
            // ignored
        }
        
        IsRunning = false;
    }

    protected virtual Task OnInitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnDeinitializeAsync()
    {
        return Task.CompletedTask;
    }
}

public abstract class ProviderShellPageViewModel: ShellPageViewModel
{
    protected ITorrentProvider? Provider;

    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();
        
        string url = (string)SettingEntries.Url.Value;
        string rpcPath = (string)SettingEntries.RpcPath.Value;
        string login = (string)SettingEntries.Login.Value;
        string password = (string)SettingEntries.Password.Value;
        int port = (int)SettingEntries.Port.Value;
        
        string fullUrl = $"{url}:{port}/{rpcPath}";
        Provider = TorrentProviderFactory.CreateProvider<TransmissionTorrentProvider>(fullUrl, login, password);
    }
}