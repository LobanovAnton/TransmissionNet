using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Shell;

public class MainShellViewModel: ProviderShellPageViewModel
{
    private static readonly SessionField[] InitFields =
    [
        SessionField.Version, SessionField.Units, SessionField.DownloadDir
    ];

    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();

        MemoryUnits = 0;
        SizeUnits = 0;
        SpeedUnits = 0;
        FreeSpace = 0;

        if (Provider != null)
        {
            SessionModel model = await Provider.GetSessionModelAsync(InitFields);
            MemoryUnits = model.MemoryUnits;
            SizeUnits = model.SizeUnits;
            SpeedUnits = model.SpeedUnits;
            FreeSpace = await Provider.GetFreeSpaceAsync(model.CompletePath);
        }
    }

    public int MemoryUnits
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public int SizeUnits
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public int SpeedUnits
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public long FreeSpace
    {
        get;
        private set
        {
            field = value;
            OnPropertyChanged();
        }
    }
}