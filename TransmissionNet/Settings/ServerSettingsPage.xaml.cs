namespace TransmissionNet.Settings;

public partial class ServerSettingsPage : ContentPage
{
    public ServerSettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ServerSettingsViewModel viewModel = (ServerSettingsViewModel)BindingContext;
        _ = viewModel.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ServerSettingsViewModel viewModel = (ServerSettingsViewModel)BindingContext;
        _ = viewModel.DeinitializeAsync();
    }
}
