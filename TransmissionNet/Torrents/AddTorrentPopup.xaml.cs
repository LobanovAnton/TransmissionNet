using TransmissionNet.TorrentProviders;
using static Microsoft.Maui.Controls.Shell;

namespace TransmissionNet.Torrents;

public partial class AddTorrentPopup
{
    private readonly TaskCompletionSource<AddTorrentOptions?> _tcs = new();

    public AddTorrentPopup()
    {
        InitializeComponent();
    }

    public async Task<AddTorrentOptions?> Show(string downloadDirectory)
    {
        DirectoryEntry.Text = downloadDirectory;
        await Current.Navigation.PushModalAsync(this);
        return await _tcs.Task;
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        _tcs.TrySetResult(null);
        Navigation.PopModalAsync();
    }

    private void OnAddClicked(object? sender, EventArgs e)
    {
        _tcs.TrySetResult(new AddTorrentOptions
        {
            DownloadDirectory = DirectoryEntry.Text,
            Paused = PausedSwitch.IsToggled,
            SequentialDownload = SequentialSwitch.IsToggled
        });
        Navigation.PopModalAsync();
    }
}
