namespace TransmissionNet.Settings;

public class AppSettingsViewModel : SettingsViewModel
{
    protected override async Task OnInitializeAsync()
    {
        Version = AppInfo.Current.VersionString;
        
        if (SettingsCollection.Count > 0) return;
        
        await base.OnInitializeAsync();
        
        SettingViewModel[] settings = SettingViewModels.ApplicationSettings;
        foreach (SettingViewModel setting in settings)
            SettingsCollection.Add(setting);
    }
    
    protected override async Task OnDeinitializeAsync()
    {
        await SettingEntries.SaveEntries(SettingsType.Application);
        
        await base.OnDeinitializeAsync();
    }
}