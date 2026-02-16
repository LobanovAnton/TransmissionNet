using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Settings;

public class ServerSettingsViewModel: SettingsViewModel
{
    private static readonly SessionField[] VersionFields =
    [
        SessionField.Version
    ];

    private CancellationTokenSource? _portTestCts;
    private bool? _isPortOpen;

    public bool? IsPortOpen
    {
        get => _isPortOpen;
        private set
        {
            if (_isPortOpen == value) return;
            _isPortOpen = value;
            OnPropertyChanged();
        }
    }

    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();

        if (Provider == null)
            return;

        SessionModel model = await Provider.GetSessionModelAsync(VersionFields);
        Version = model.Version;

        await SettingEntries.LoadEntries(SettingsType.Server);
        SettingViewModel[] settings = SettingViewModels.TorrentSettings;
        foreach (SettingViewModel setting in settings)
            SettingsCollection.Add(setting);

        SettingViewModels.PeerPort.ValueChanged += OnPeerPortChanged;
        await RunPortTestAsync();
    }

    protected override async Task OnDeinitializeAsync()
    {
        SettingViewModels.PeerPort.ValueChanged -= OnPeerPortChanged;
        _portTestCts?.Cancel();
        _portTestCts?.Dispose();
        _portTestCts = null;

        if (SettingsCollection.Count > 0)
            await SettingEntries.SaveEntries(SettingsType.Server);

        SettingsCollection.Clear();
    }

    private void OnPeerPortChanged()
    {
        _portTestCts?.Cancel();
        _portTestCts?.Dispose();
        _portTestCts = new CancellationTokenSource();
        _ = SavePortAndTestWithDelay(_portTestCts.Token);
    }

    private async Task SavePortAndTestWithDelay(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(500, cancellationToken);
            await SavePortAndTestAsync(cancellationToken);
        }
        catch (OperationCanceledException) { }
    }

    private async Task SavePortAndTestAsync(CancellationToken cancellationToken)
    {
        if (Provider == null)
            return;

        int port = (int)SettingEntries.PeerPort.Value;
        await Provider.SetSessionSettingsAsync(new SessionSettingsModel { PeerPort = port }, cancellationToken);
        IsPortOpen = await Provider.PortTestAsync(cancellationToken);
    }

    private async Task RunPortTestAsync(CancellationToken cancellationToken = default)
    {
        if (Provider == null)
            return;

        try
        {
            IsPortOpen = await Provider.PortTestAsync(cancellationToken);
        }
        catch (Exception)
        {
            IsPortOpen = null;
        }
    }
}
