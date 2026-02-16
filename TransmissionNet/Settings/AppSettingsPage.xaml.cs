namespace TransmissionNet.Settings;

public partial class AppSettingsPage : ContentPage
{
    public AppSettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AppSettingsViewModel viewModel = (AppSettingsViewModel)BindingContext;
        _ = viewModel.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        AppSettingsViewModel viewModel = (AppSettingsViewModel)BindingContext;
        _ = viewModel.DeinitializeAsync();
    }
}
