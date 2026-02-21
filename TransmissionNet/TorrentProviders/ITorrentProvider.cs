namespace TransmissionNet.TorrentProviders;

public static class TorrentProviderFactory
{
    public static ITorrentProvider CreateProvider<T>(string url, string username, string password) where T : ITorrentProvider
    {
        return CreateProvider(typeof(T), url, username, password);
    }

    public static ITorrentProvider CreateProvider(Type type, string url, string username, string password)
    {
        switch (type.Name)
        {
            case nameof(TransmissionTorrentProvider):
                return new TransmissionTorrentProvider(url, username, password);
            default:
                throw new ArgumentException($"Unknown provider type: {type.Name}");
        }
    }
}

public interface ITorrentProvider
{
    Task<long> GetFreeSpaceAsync(string path, CancellationToken cancellationToken = default);

    Task SetTorrentLocationAsync(int id, string destinationPath, CancellationToken cancellationToken = default);

    Task<SessionModel> GetSessionModelAsync(SessionField[] fields, CancellationToken cancellationToken = default);

    Task SetSessionSettingsAsync(SessionSettingsModel model, CancellationToken cancellationToken = default);

    Task<StatisticModel> GetStatisticModelAsync(CancellationToken cancellationToken = default);

    Task<TorrentModel[]> GetTorrentModelAsync(TorrentField[] fields, int[]? ids = null, CancellationToken cancellationToken = default);

    Task SetTorrentSettingsAsync(TorrentSettingsModel settings, CancellationToken cancellationToken = default);

    Task AddTorrentAsync(string metaInfo, AddTorrentOptions? options = null, CancellationToken cancellationToken = default);

    Task StartNowTorrentAsync(int id, CancellationToken cancellationToken = default);

    Task StartTorrentAsync(int id, CancellationToken cancellationToken = default);

    Task StopTorrentAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteTorrentAsync(int id, bool deleteFiles, CancellationToken cancellationToken = default);

    Task VerifyTorrentAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteAllAsync(int[] ids, CancellationToken cancellationToken = default);

    Task StartAllAsync(int[] ids, CancellationToken cancellationToken = default);

    Task StopAllAsync(int[] ids, CancellationToken cancellationToken = default);

    Task<bool> PortTestAsync(CancellationToken cancellationToken = default);
}
