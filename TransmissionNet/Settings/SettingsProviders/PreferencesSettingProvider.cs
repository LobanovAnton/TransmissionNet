namespace TransmissionNet.Settings.SettingsProviders;

public class PreferencesSettingsProvider
{
    public async Task LoadValuesAsync(SettingEntry[] settings)
    {
        await Task.Run(() =>
        {
            foreach (SettingEntry setting in settings)
            {
                try
                {
                    switch (setting.DefaultValue.GetType())
                    {
                        case { } type when type == typeof(string):
                            setting.Value = Preferences.Get(setting.Key, (string)setting.DefaultValue);
                            break;
                        case { } type when type == typeof(int):
                            setting.Value = Preferences.Get(setting.Key, (int)setting.DefaultValue);
                            break;
                        case { } type when type == typeof(bool):
                            setting.Value = Preferences.Get(setting.Key, (bool)setting.DefaultValue);
                            break;
                    }
                }
                catch (Exception)
                {
                    setting.Value = setting.DefaultValue;
                }
                setting.IsValid = true;
            }
        });
    }

    public async Task SaveValuesAsync(SettingEntry[] settings)
    {
        await Task.Run(() =>
        {
            foreach (SettingEntry setting in settings)
            {
                if (setting.IsValid)
                {
                    switch (setting.DefaultValue.GetType())
                    {
                        case { } type when type == typeof(string):
                            Preferences.Set(setting.Key, (string)setting.Value);
                            break;
                        case { } type when type == typeof(int):
                            Preferences.Set(setting.Key, (int)setting.Value);
                            break;
                        case { } type when type == typeof(bool):
                            Preferences.Set(setting.Key, (bool)setting.Value);
                            break;
                    }
                }
            }
        });
    }
}