using TransmissionNet.Services;
using TransmissionNet.TorrentProviders;
using static Microsoft.Maui.Controls.Shell;

namespace TransmissionNet.Torrents;

public partial class AddTorrentPopup
{
    private readonly TaskCompletionSource<AddTorrentOptions?> _tcs = new();
    private CancellationTokenSource? _cts;

    public AddTorrentPopup()
    {
        InitializeComponent();
    }

    public async Task<AddTorrentOptions?> Show(string downloadDirectory, string torrentName, IMovieService? movieService)
    {
        DirectoryEntry.Text = downloadDirectory;
        await Current.Navigation.PushModalAsync(this);
        _cts = new CancellationTokenSource();
        if (movieService != null) 
            _ =  LoadMovieInfoAsync(movieService, torrentName, _cts.Token);
        return await _tcs.Task;
    }

    private async Task LoadMovieInfoAsync(IMovieService movieService, string torrentName, CancellationToken cancellationToken)
    {
        LoadingIndicator.IsVisible = true;
        MoviePanel.IsVisible = true;

        try
        {
            MovieInfo? info = await movieService.SearchAsync(torrentName, cancellationToken);
            if (info == null)
            {
                MoviePanel.IsVisible = false;
                return;
            }

            TitleLabel.Text = string.IsNullOrEmpty(info.Year) ? info.Title : $"{info.Title} ({info.Year})";
            GenresLabel.Text = string.Join(", ", info.Genres);
            DescriptionLabel.Text = info.Description;

            string rating = "";
            if (!string.IsNullOrEmpty(info.RatingKp))
                rating += $"KP: {info.RatingKp}";
            if (!string.IsNullOrEmpty(info.RatingImdb))
                rating += (rating.Length > 0 ? "  " : "") + $"IMDb: {info.RatingImdb}";
            RatingLabel.Text = rating;

            if (!string.IsNullOrEmpty(info.Director))
            {
                DirectorLabel.Text = $"Режиссёр: {info.Director}";
                if (!string.IsNullOrEmpty(info.DirectorPhotoUrl))
                    DirectorPhoto.Source = ImageSource.FromUri(new Uri(info.DirectorPhotoUrl));
                else
                    DirectorPhoto.IsVisible = false;
            }
            else
            {
                DirectorPanel.IsVisible = false;
            }

            if (!string.IsNullOrEmpty(info.PosterUrl))
                PosterImage.Source = ImageSource.FromUri(new Uri(info.PosterUrl));
            else
                PosterImage.IsVisible = false;

            if (info.Actors.Length > 0)
            {
                ActorsHeader.IsVisible = true;
                foreach (ActorInfo actor in info.Actors)
                {
                    Grid actorGrid = new() { ColumnDefinitions = [new(44), new(GridLength.Star)], ColumnSpacing = 8 };

                    Image photo = new() { WidthRequest = 40, HeightRequest = 40, Aspect = Aspect.AspectFill, VerticalOptions = LayoutOptions.Center };
                    photo.Clip = new Microsoft.Maui.Controls.Shapes.EllipseGeometry { Center = new Point(20, 20), RadiusX = 20, RadiusY = 20 };
                    if (!string.IsNullOrEmpty(actor.PhotoUrl))
                        photo.Source = ImageSource.FromUri(new Uri(actor.PhotoUrl));
                    Grid.SetColumn(photo, 0);
                    actorGrid.Children.Add(photo);

                    Label actorLabel = new()
                    {
                        Text = string.IsNullOrEmpty(actor.Role) ? actor.Name : $"{actor.Name} — {actor.Role}",
                        FontSize = 14,
                        VerticalOptions = LayoutOptions.Center
                    };
                    Grid.SetColumn(actorLabel, 1);
                    actorGrid.Children.Add(actorLabel);

                    ActorsPanel.Children.Add(actorGrid);
                }
            }

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            MoviePanel.IsVisible = true;
            MoviePanel.Opacity = 0;
            await MoviePanel.FadeToAsync(1, 500);
        }
        catch (OperationCanceledException) { }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        _cts?.Cancel();
        _tcs.TrySetResult(null);
        Navigation.PopModalAsync();
    }

    private void OnAddClicked(object? sender, EventArgs e)
    {
        _cts?.Cancel();
        _tcs.TrySetResult(new AddTorrentOptions
        {
            DownloadDirectory = DirectoryEntry.Text,
            Paused = PausedSwitch.IsToggled,
            SequentialDownload = SequentialSwitch.IsToggled
        });
        Navigation.PopModalAsync();
    }
}
