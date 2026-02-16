using TransmissionNet.Shell;

namespace TransmissionNet.MainApp;

public class WindowCreator : IWindowCreator
{
    public Window CreateWindow(Application app, IActivationState? activationState)
    {
        return new Window(new MainShell());
    }
}

public partial class App
{
    public App()
    {
        InitializeComponent();
    }
}