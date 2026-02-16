namespace TransmissionNet.Triggers;

public class ImageButtonIsPressed : TriggerAction<ImageButton>
{
    public static readonly ImageButtonIsPressed Instance = new();
    
    protected override async void Invoke(ImageButton sender)
    {
        if (sender.IsPressed)
        {
            await sender.ScaleToAsync(1.2, 250, Easing.CubicInOut);
            await sender.ScaleToAsync(1.0, 250, Easing.CubicInOut);
        }
    }
}

public class ButtonIsPressed : TriggerAction<Button>
{
    public static readonly ButtonIsPressed Instance = new();
    
    protected override async void Invoke(Button sender)
    {
        if (sender.IsPressed)
        {
            await sender.ScaleToAsync(1.2, 250, Easing.CubicInOut);
            await sender.ScaleToAsync(1.0, 250, Easing.CubicInOut);
        }
    }
}