namespace TransmissionNet.Torrents;

public partial class TorrentsPage
{
    public TorrentsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TorrentsViewModel viewModel = (TorrentsViewModel)BindingContext;
        _ = viewModel.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        TorrentsViewModel viewModel = (TorrentsViewModel)BindingContext;
        _ = viewModel.DeinitializeAsync();
    }
}
