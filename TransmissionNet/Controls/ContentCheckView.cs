namespace TransmissionNet.Controls;

public class ContentCheckView : ContentView
{
    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(VisualElement), false,
        BindingMode.OneWay, null, CheckedPropertyChanged);

    public event EventHandler? CheckedChanged;

    private static void CheckedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        ((ContentCheckView)bindable).CheckedChanged?.Invoke(bindable, EventArgs.Empty);
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
}