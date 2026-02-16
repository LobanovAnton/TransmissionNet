using System.Collections.ObjectModel;
using TransmissionNet.Shell;
using TransmissionNet.TorrentProviders;
using TransmissionNet.Extensions;
using TransmissionNet.Settings;

namespace TransmissionNet.Torrents;

public class TorrentsViewModel: UpdatableShellPageViewModel
{
    private static readonly TorrentField[] ListFields =
    [
        TorrentField.Id, TorrentField.Name, TorrentField.TotalSize,
        TorrentField.Status, TorrentField.PercentDone, TorrentField.RecheckProgress,
        TorrentField.RateDownload, TorrentField.RateUpload, TorrentField.UploadRatio,
        TorrentField.LeftUntilDone, TorrentField.SecondsDownloading,
        TorrentField.AddedDate, TorrentField.QueuePosition, TorrentField.DownloadDir
    ];

    private readonly Func<TorrentModel, bool> _filterFunc;
    private readonly Func<TorrentModel, bool> _pathFilterFunc;
    private StatisticModel _statistic = StatisticModel.Empty;

    private FilterState _filterState;
    private SortRule _sortRule;
    private SortMode _sortMode;
    private string _path = string.Empty;
    private int _pathIndex;

    public TorrentsViewModel()
    {
        Tap = new Command(OnTap);
        ButtonPressed = new Command(OnClick);
        StartNow = new Command(OnStartNow);
        Add = new Command(OnAdd);
        Delete = new Command(OnDelete);
        DeleteFiles = new Command(OnDeleteFiles);
        Verify = new Command(OnVerify);
        DeleteAll = new Command(OnDeleteAll);
        StartAll = new Command(OnStartAll);
        StopAll = new Command(OnStopAll);

        _filterFunc = m => FilterFunc(_filterState, m);
        _pathFilterFunc = m => PathFilterFunc(_pathIndex, _path, m);
    }

    public StatisticModel Statistic
    {
        get => _statistic;
        private set
        {
            _statistic = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<TorrentViewModel> Items { get; } = [];

    protected override Task OnInitializeAsync()
    {
        _filterState = FilterSortSettings.FilterState;
        _sortRule = FilterSortSettings.SortRule;
        _sortMode = FilterSortSettings.SortMode;
        _pathIndex = FilterSortSettings.PathIndex;
        _path = FilterSortSettings.Path;

        return base.OnInitializeAsync();
    }

    private async void OnAdd()
    {
        try
        {
            if (Provider == null)
                return;

            Dictionary<DevicePlatform, IEnumerable<string>> dictionary = new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, [".torrent"] },
                { DevicePlatform.Android, ["application/x-bittorrent"] },
                { DevicePlatform.MacCatalyst, ["torrent"] },
                { DevicePlatform.iOS, ["com.transmission.torrent"] }
            };
            PickOptions options = new PickOptions
            {
                FileTypes = new FilePickerFileType(dictionary),
                PickerTitle = "Torrents"
            };

            IEnumerable<FileResult?> results = await FilePicker.PickMultipleAsync(options);
            foreach (FileResult? fileResult in results)
            {
                if (fileResult != null)
                {
                    await using (Stream stream = await fileResult.OpenReadAsync())
                    {
                        byte[] bytes = new byte[stream.Length];
                        await stream.ReadExactlyAsync(bytes);
                        string metaInfo = Convert.ToBase64String(bytes);
                        await Provider.AddTorrentAsync(metaInfo);
                    }

                    if ((bool)SettingEntries.DeleteTorrentFile.Value && File.Exists(fileResult.FullPath))
                        File.Delete(fileResult.FullPath);
                }
            }
            _ = UpdateAsync();
        }
        catch (Exception)
        {
            // The user canceled or something went wrong
        }
    }

    private async void OnStartNow(object arg)
    {
        try
        {
            if (Provider == null)
                return;

            TorrentViewModel vm = (TorrentViewModel)arg;
            await Provider.StartNowTorrentAsync(vm.Id);
            _ = UpdateAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private void OnDelete(object arg)
    {
        DeleteImpl(arg, false);
    }

    private void OnDeleteFiles(object arg)
    {
        DeleteImpl(arg, true);
    }

    private async void DeleteImpl(object arg, bool deleteFiles)
    {
        try
        {
            if (Provider == null)
                return;

            TorrentViewModel vm = (TorrentViewModel)arg;
            await Provider.DeleteTorrentAsync(vm.Id, deleteFiles);
            _ = UpdateAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private async void OnVerify(object arg)
    {
        try
        {
            if (Provider == null)
                return;

            TorrentViewModel vm = (TorrentViewModel)arg;
            await Provider.VerifyTorrentAsync(vm.Id);
            _ = UpdateAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private async void OnDeleteAll()
    {
        try
        {
            if (Provider == null)
                return;

            await Provider.DeleteAllAsync(Items.Select(x => x.Id).ToArray());
            _ = UpdateAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private async void OnStartAll()
    {
        try
        {
            if (Provider == null)
                return;

            await Provider.StartAllAsync(Items.Select(x => x.Id).ToArray());
            _ = UpdateAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private async void OnStopAll()
    {
        try
        {
            if (Provider == null)
                return;

            await Provider.StopAllAsync(Items.Select(x => x.Id).ToArray());
            _ = UpdateAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private async void OnClick(object arg)
    {
        try
        {
            if (Provider == null)
                return;

            TorrentViewModel vm = (TorrentViewModel)arg;
            switch (vm.Status)
            {
                case TorrentModel.State.Seeding:
                case TorrentModel.State.Downloading:
                case TorrentModel.State.Verifying:
                case TorrentModel.State.WaitDownload:
                case TorrentModel.State.WaitVerify:
                case TorrentModel.State.WaitingSeed:
                    await Provider.StopTorrentAsync(vm.Id);
                    break;
                case TorrentModel.State.Stopped:
                    await Provider.StartTorrentAsync(vm.Id);
                    break;
            }
            _ = UpdateAsync();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private async void OnTap(object arg)
    {
        try
        {
            TorrentViewModel vm = (TorrentViewModel)arg;

            FilePageViewModel fileViewModel = (FilePageViewModel)Application.Current!.Resources[nameof(FilePageViewModel)];
            fileViewModel.Torrent = vm;

            await Microsoft.Maui.Controls.Shell.Current.GoToAsync("//torrents/file");
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public Command Tap { get; private set; }

    public Command ButtonPressed { get; private set; }

    public Command StartNow { get; private set; }

    public Command Add { get; private set; }

    public Command Delete { get; private set; }

    public Command DeleteFiles { get; private set; }

    public Command Verify { get; private set; }

    public Command DeleteAll { get; private set; }

    public Command StartAll { get; private set; }

    public Command StopAll { get; private set; }

    protected override async Task OnUpdateAsync(CancellationToken cancellationToken)
    {
        if (Provider == null)
            return;

        Task<StatisticModel> statTask = Provider.GetStatisticModelAsync(cancellationToken);
        Task<TorrentModel[]> torrentsTask = Provider.GetTorrentModelAsync(ListFields, cancellationToken: cancellationToken);
        TorrentModel[] models;

        try
        {
            await Task.WhenAll(statTask, torrentsTask);

            Statistic = statTask.Result;
            models = torrentsTask.Result;
        }
        catch (Exception)
        {
            Statistic = StatisticModel.Empty;
            models = [];
        }

        TorrentModel[] filtered = models.Where(_pathFilterFunc).Where(_filterFunc).ToArray();

        TorrentModel[] sorted = OrderByMode(filtered, _sortMode);

        RefreshCollection(sorted);

        UpdateCollection(sorted);
    }

    private static bool FilterFunc(FilterState state, TorrentModel model)
    {
        switch (state)
        {
            case FilterState.All:
                return true;
            case FilterState.Active:
                return model.DownloadRate != 0 || model.UploadRate != 0;
            case FilterState.ActiveDownloading:
                return model.DownloadRate != 0;
            case FilterState.ActiveSeeding:
                return model.UploadRate != 0;
            case FilterState.Downloading:
                return model.Status == TorrentModel.State.Downloading;
            case FilterState.Seeding:
                return model.Status == TorrentModel.State.Seeding;
            case FilterState.Waiting:
                return model.Status == TorrentModel.State.WaitDownload ||
                       model.Status == TorrentModel.State.WaitVerify ||
                       model.Status == TorrentModel.State.WaitingSeed;
            case FilterState.Stopped:
                return model.Status == TorrentModel.State.Stopped;
            default:
                return false;
        }
    }

    private static int QueuePosition(TorrentModel model)
    {
        return model.QueuePosition;
    }

    private static int Status(TorrentModel model)
    {
        return (int)model.Status;
    }

    private static string Name(TorrentModel model)
    {
        return model.Name;
    }

    private static long Size(TorrentModel model)
    {
        return model.TotalSize;
    }

    private static double Progress(TorrentModel model)
    {
        return model.Progress;
    }

    private static int DownloadSpeed(TorrentModel model)
    {
        return model.DownloadRate;
    }

    private static int RemainingTime(TorrentModel model)
    {
        return model.SecondsDownloading;
    }

    private static int UploadSpeed(TorrentModel model)
    {
        return model.UploadRate;
    }

    private static double Ratio(TorrentModel model)
    {
        return model.UploadRatio;
    }

    private static long DateAdded(TorrentModel model)
    {
        return model.DateAdded;
    }

    private TorrentModel[] OrderByMode(TorrentModel[] models, SortMode sortMode)
    {
        IEnumerable<TorrentModel> sorted;

        switch (_sortRule)
        {
            case SortRule.QueuePosition:
                sorted = models.OrderByMode(sortMode, QueuePosition);
                break;
            case SortRule.State:
                sorted = models.OrderByMode(sortMode, Status);
                break;
            case SortRule.Name:
                sorted = models.OrderByMode(sortMode, Name);
                break;
            case SortRule.Size:
                sorted = models.OrderByMode(sortMode, Size);
                break;
            case SortRule.Progress:
                sorted = models.OrderByMode(sortMode, Progress);
                break;
            case SortRule.DownloadSpeed:
                sorted = models.OrderByMode(sortMode, DownloadSpeed);
                break;
            case SortRule.RemainingTime:
                sorted = models.OrderByMode(sortMode, RemainingTime);
                break;
            case SortRule.UploadSpeed:
                sorted = models.OrderByMode(sortMode, UploadSpeed);
                break;
            case SortRule.Ratio:
                sorted = models.OrderByMode(sortMode, Ratio);
                break;
            case SortRule.DateAdded:
                sorted = models.OrderByMode(sortMode, DateAdded);
                break;
            default:
                sorted = models;
                break;
        }

        return sorted.ToArray();
    }

    private void RefreshCollection(TorrentModel[] models)
    {
        for (int i = 0; i < Items.Count; ++i)
        {
            if (models.FirstOrDefault(item => Items[i].Id == item.Id) == null)
                Items.RemoveAt(i--);
        }
    }

    private void UpdateCollection(TorrentModel[] models)
    {
        for (int i = 0; i < models.Length; i++)
        {
            TorrentModel model = models[i];
            int index = Items.FindIndex(vm => vm.Id == model.Id);

            if (index >= 0)
            {
                TorrentViewModel vm = Items[index];
                vm.Update(model);

                if (index != i)
                {
                    Items.RemoveAt(index);
                    Items.Insert(i, vm);
                }
            }
            else
                Items.Insert(i, new TorrentViewModel(model));
        }
    }

    private static bool PathFilterFunc(int pathIndex, string path, TorrentModel model)
    {
        if (pathIndex == 0)
            return true;

        return String.Equals(path, model.DownloadDir, StringComparison.OrdinalIgnoreCase);
    }
}
