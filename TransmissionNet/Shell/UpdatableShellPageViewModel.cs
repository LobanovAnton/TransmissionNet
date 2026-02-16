using TransmissionNet.Settings;

namespace TransmissionNet.Shell;

public abstract class UpdatableShellPageViewModel: ProviderShellPageViewModel
{
    private readonly IDispatcherTimer _timer;
    private CancellationTokenSource? _cancellationTokenSource;

    protected UpdatableShellPageViewModel()
    {
        _timer = Dispatcher.CreateTimer();
        _timer.IsRepeating = false;
        _timer.Tick += TimerOnTick;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        _ = UpdateAsync();
    }

    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();

        _cancellationTokenSource = new CancellationTokenSource();

        int updateInterval = (int)SettingEntries.UpdateInterval.Value;
        _timer.Interval = TimeSpan.FromSeconds(updateInterval);

        await UpdateAsync();
    }

    protected override Task OnDeinitializeAsync()
    {
        _timer.Stop();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        return base.OnDeinitializeAsync();
    }

    protected async Task UpdateAsync()
    {
        if (!IsInitialized) return;

        IsRunning = true;

        try
        {
            CancellationToken cancellationToken = _cancellationTokenSource?.Token ?? CancellationToken.None;
            cancellationToken.ThrowIfCancellationRequested();
            
            await OnUpdateAsync(cancellationToken);
            
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        IsRunning = false;

        _timer.Start();
    }

    protected abstract Task OnUpdateAsync(CancellationToken cancellationToken);
}
