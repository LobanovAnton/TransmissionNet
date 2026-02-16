using System.ComponentModel;
using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Torrents;

public partial class FilePage
{
    public FilePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FilePageViewModel viewModel = (FilePageViewModel)BindingContext;
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _ = viewModel.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        FilePageViewModel viewModel = (FilePageViewModel)BindingContext;
        viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _ = viewModel.DeinitializeAsync();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(FilePageViewModel.Pieces))
            return;
        
        if (sender is not FilePageViewModel viewModel)
            return;

        foreach (PieceMapView pieceMapView in this.GetVisualTreeDescendants().OfType<PieceMapView>())
        {
            if (pieceMapView.IsVisible)
            {
                pieceMapView.Availability = viewModel.Availability;
                pieceMapView.Pieces = viewModel.Pieces;
            }
        }
    }

    private void OnPriorityClicked(object? sender, EventArgs e)
    {
        if (sender is Button { BindingContext: FileViewModel file } button
            && Enum.TryParse(button.ClassId, out FilePriority priority))
            file.Priority = priority;
    }

    private async void OnFileTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            if (sender is not Grid { Parent: VerticalStackLayout layout, BindingContext: FileViewModel file })
                return;

            if (layout.Children.Count > 1 && layout.Children[1] is PieceMapView pieceMapView)
            {
                FilePageViewModel viewModel = (FilePageViewModel)BindingContext;
                pieceMapView.Availability = viewModel.Availability;
                pieceMapView.Pieces = viewModel.Pieces;
                if (!file.IsExpanded)
                {
                    file.IsExpanded = true;
                    pieceMapView.Opacity = 0;
                    await pieceMapView.FadeToAsync(1, 250, Easing.CubicIn);
                }
                else
                {
                    await pieceMapView.FadeToAsync(0, 250, Easing.CubicIn);
                    file.IsExpanded = false;
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
