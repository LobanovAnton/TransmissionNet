using TransmissionNet.Shell;
using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Torrents;

public class StatisticsViewModel: UpdatableShellPageViewModel
{
    private StatisticModel _statistic = StatisticModel.Empty;

    public StatisticModel Statistic
    {
        get => _statistic;
        private set
        {
            _statistic = value;
            OnPropertyChanged();
        }
    }
    
    protected override async Task OnUpdateAsync(CancellationToken cancellationToken)
    {
        if (Provider == null)
            return;

        try
        {
            Statistic = await Provider.GetStatisticModelAsync(cancellationToken);
        }
        catch (Exception)
        {
            Statistic = StatisticModel.Empty;
        }
    }
}