namespace TransmissionNet.Controls;

public class AnimatedProgressBar : ProgressBar
{
    public static readonly BindableProperty AnimatedProgressProperty =
        BindableProperty.Create(nameof(AnimatedProgress), typeof(double), typeof(AnimatedProgressBar), 0.0,
            propertyChanged: OnAnimatedProgressChanged);

    public double AnimatedProgress
    {
        get => (double)GetValue(AnimatedProgressProperty);
        set => SetValue(AnimatedProgressProperty, value);
    }

    private static void OnAnimatedProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        AnimatedProgressBar bar = (AnimatedProgressBar)bindable;
        if (oldValue == newValue)
            return;
        bar.ProgressTo((double)newValue, 1000, Easing.CubicInOut);
    }
}
