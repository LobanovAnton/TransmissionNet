using System.ComponentModel;

namespace TransmissionNet.Shell;

public partial class MainShell : Microsoft.Maui.Controls.Shell
{
    public MainShell()
    {
        InitializeComponent();
        
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == FlyoutIsPresentedProperty.PropertyName)
        {
            MainShellViewModel model = (MainShellViewModel)BindingContext;
            _ = model.InitializeAsync();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MainShellViewModel model = (MainShellViewModel)BindingContext;
        _ = model.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MainShellViewModel model = (MainShellViewModel)BindingContext;
        _ = model.DeinitializeAsync();
    }
}