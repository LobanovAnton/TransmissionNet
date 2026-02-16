using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Settings.SettingsProviders;

public class TorrentSettingsProvider
{
    private static readonly SessionField[] AllFields = Enum.GetValues<SessionField>();

    private ITorrentProvider? _torrentProvider;

    public void Initialize(string url, string login, string password)
    {
        _torrentProvider = TorrentProviderFactory.CreateProvider<TransmissionTorrentProvider>(url, login, password);
    }

    public async Task LoadValuesAsync(SettingEntry[] settings)
    {
        if (_torrentProvider != null)
        {
            try
            {
                SessionModel session = await _torrentProvider.GetSessionModelAsync(AllFields);

                foreach (SettingEntry settingEntry in settings)
                {
                    settingEntry.Value = session.GetType().GetProperty(settingEntry.Key)?.GetValue(session) ??
                                         settingEntry.DefaultValue;
                    settingEntry.IsValid = true;
                }
            }
            catch (Exception)
            {
                foreach (SettingEntry settingEntry in settings)
                {
                    settingEntry.Value = settingEntry.DefaultValue;
                    settingEntry.IsValid = true;
                }
            }
        }
    }

    public async Task SaveValuesAsync(SettingEntry[] settings)
    {
        if (_torrentProvider != null)
        {
            SessionSettingsModel model = new();

            foreach (SettingEntry settingEntry in settings)
            {
                if (settingEntry.IsValid)
                    model.GetType().GetProperty(settingEntry.Key)?.SetValue(model, settingEntry.Value);
            }

            try
            {
                await _torrentProvider.SetSessionSettingsAsync(model);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}