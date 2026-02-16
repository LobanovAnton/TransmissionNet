namespace TransmissionNet.Torrents;

public partial class StatisticsPage
{
    public StatisticsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StatisticsViewModel viewModel = (StatisticsViewModel)BindingContext;
        _ = viewModel.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StatisticsViewModel viewModel = (StatisticsViewModel)BindingContext;
        _ = viewModel.DeinitializeAsync();
    }
}