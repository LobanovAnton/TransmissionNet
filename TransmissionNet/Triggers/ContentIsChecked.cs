using TransmissionNet.Controls;

namespace TransmissionNet.Triggers;

public class ContentIsChecked : TriggerAction<ContentCheckView>
{
    public static readonly ContentIsChecked Instance = new();
    
    protected override async void Invoke(ContentCheckView sender)
    {
        if (sender.IsChecked)
        {
            Task t1 = sender.FadeToAsync(1);
            Task t2 = new Task(async void () =>
            {
                //await sender.ScaleToAsync(0, 250, Easing.CubicOut);
                await sender.ScaleToAsync(1.0, 250, Easing.CubicOut);
            });
            t2.Start();
            await Task.WhenAll(t1, t2);
        }
        else
        {
            Task t1 = sender.ScaleToAsync(0, 250, Easing.CubicIn);
            Task t2 = sender.FadeToAsync(0);
            await Task.WhenAll(t1, t2);
        }
    }
}