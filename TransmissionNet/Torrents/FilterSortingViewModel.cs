using TransmissionNet.Extensions;
using TransmissionNet.Shell;
using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Torrents;

public enum FilterState
{
    None = -1,
    All,
    Active,
    ActiveDownloading,
    ActiveSeeding,
    Downloading,
    Seeding,
    Waiting,
    Stopped
}

public enum SortRule
{
    None = -1,
    NoSorting,
    QueuePosition,
    Name,
    State,
    Size,
    Progress,
    DownloadSpeed,
    RemainingTime,
    UploadSpeed,
    Ratio,
    DateAdded
}

public static class FilterSortSettings
{
    public static FilterState FilterState { get; set; } = FilterState.All;
    
    public static SortRule SortRule { get; set; } = SortRule.NoSorting;
    
    public static SortMode SortMode { get; set; } = SortMode.Descending;
    
    public static int PathIndex { get; set; }
    
    public static string Path { get; set; } = string.Empty;
}

public class FilterSortingViewModel: UpdatableShellPageViewModel
{
    private static readonly TorrentField[] PathFields =
    [
        TorrentField.Id, TorrentField.DownloadDir
    ];

    private FilterState _filterState = FilterState.None;
    private SortRule _sortRule = SortRule.None;
    private SortMode _sortMode;
    private string[] _filtersNames = [];
    private string[] _sortRulesNames = [];
    private string[] _defaultPath = [];
    private int _storedPathIndex = -1;
    
    private int _pathIndex = -1;
    private string _path = string.Empty;
    private string[] _paths = [];

    protected override Task OnInitializeAsync()
    {
        if (Application.Current != null)
        {
            ResourceDictionary resources = Application.Current.Resources;
            
            string[] keys = Enum.GetNames(typeof(FilterState));
            _filtersNames = new string[keys.Length - 1];
            resources.GetValuesByKeys(keys, 0, keys.Length - 1, _filtersNames);
            
            keys = Enum.GetNames(typeof(SortRule));
            _sortRulesNames = new string[keys.Length - 1];
            resources.GetValuesByKeys(keys, 0, keys.Length - 1, _sortRulesNames);
            
            if (resources.TryGetValue("AllFolders", out var result))
                _defaultPath = [(string)result];
        }
        
        FilterNames = _filtersNames;
        SortRuleNames = _sortRulesNames;
        
        if (_paths.Length == 0)
            Paths = _defaultPath;
        
        FilterState = FilterSortSettings.FilterState;
        SortRule = FilterSortSettings.SortRule;
        SortMode = FilterSortSettings.SortMode;
        PathIndex = FilterSortSettings.PathIndex;
        
        return base.OnInitializeAsync();
    }

    protected override Task OnDeinitializeAsync()
    {
        FilterSortSettings.FilterState = _filterState;
        FilterSortSettings.SortRule = _sortRule;
        FilterSortSettings.SortMode = _sortMode;
        FilterSortSettings.PathIndex = _pathIndex;
        FilterSortSettings.Path = _path;
        
        return base.OnDeinitializeAsync();
    }

    private class PathsDeferral
    {
        public required string[] DefaultPath;
        public required string[] Paths;
    }

    private static async Task ProcessPaths(PathsDeferral pathsDeferral, TorrentModel[] models)
    {
        await Task.Run(() => 
        {
            HashSet<string> pathsSet = new HashSet<string>(pathsDeferral.DefaultPath);
            
            for (int i = 0; i < models.Length; i++)
                pathsSet.Add(models[i].DownloadDir);

            if (!pathsSet.SequenceEqual(pathsDeferral.Paths))
                pathsDeferral.Paths = pathsSet.ToArray();
        });
    }

    protected override async Task OnUpdateAsync(CancellationToken cancellationToken)
    {
        if (Provider == null) return;

        PathsDeferral pathsDeferral = new PathsDeferral
        {
            DefaultPath = _defaultPath,
            Paths = _paths,
        };

        try
        {
            TorrentModel[] models = await Provider.GetTorrentModelAsync(PathFields, cancellationToken: cancellationToken);

            await ProcessPaths(pathsDeferral, models);

            Paths = pathsDeferral.Paths;

            if (_storedPathIndex >= 0)
            {
                PathIndex = _storedPathIndex;
                _storedPathIndex = -1;
            }
        }
        catch (Exception)
        {
            if (_pathIndex >= 0)
                _storedPathIndex = _pathIndex;
            PathIndex = -1;
        }
    }

    public FilterState FilterState
    {
        get => _filterState;
        set
        {
            if (_filterState == value) return;
            _filterState = value;
            OnPropertyChanged();
        }
    }
    
    public string[] FilterNames
    {
        get => _filtersNames;
        set
        {
            _filtersNames = value;
            OnPropertyChanged();
        }
    }

    public SortRule SortRule
    {
        get => _sortRule;
        set
        {
            if (_sortRule == value) return;
            _sortRule = value;
            OnPropertyChanged();
        }
    }
    
    public string[] SortRuleNames
    {
        get => _sortRulesNames;
        set
        {
            _sortRulesNames = value;
            OnPropertyChanged();
        }
    }

    public SortMode SortMode
    {
        get => _sortMode;
        set
        {
            if (_sortMode == value) return;
            _sortMode = value;
            OnPropertyChanged();
        }
    }

    public int PathIndex
    {
        get => _pathIndex;
        set
        {
            if (_pathIndex == value) return;
            _pathIndex = value;
            if (_pathIndex > 0 && _pathIndex < _paths.Length)
                _path = _paths[_pathIndex];
            OnPropertyChanged();
        }
    }
    
    public string[] Paths
    {
        get => _paths;
        set
        {
            if (_paths == value) return;
            _paths = value;
            OnPropertyChanged();
        }
    }
}