using TransmissionNet.Extensions;

namespace TransmissionNet.Torrents;

public partial class FilterSorting
{
    public FilterSorting()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FilterSortingViewModel model = (FilterSortingViewModel)BindingContext;
        _ = model.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        FilterSortingViewModel model = (FilterSortingViewModel)BindingContext;
        _ = model.DeinitializeAsync();
    }

    private void SortModeOnClicked(object? sender, EventArgs e)
    {
        FilterSortingViewModel model = (FilterSortingViewModel)BindingContext;
        model.SortMode = model.SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
    }
}