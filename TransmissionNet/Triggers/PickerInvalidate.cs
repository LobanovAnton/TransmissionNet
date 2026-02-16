using CommunityToolkit.Maui.Behaviors;
using TransmissionNet.Settings;

namespace TransmissionNet.Triggers;

public class PickerInvalidate : TriggerAction<Picker>
{
    public static readonly PickerInvalidate Instance = new();
    
    protected override void Invoke(Picker sender)
    {
        sender.IsVisible = false;
        sender.IsVisible = true;
    }
}

public class EntryTextChanged : TriggerAction<Entry>
{
    public static readonly EntryTextChanged Instance = new();
    
    protected override void Invoke(Entry sender)
    {
        SettingViewModel model = (SettingViewModel)sender.BindingContext;
        if (model == null) return;
        
        bool isValid = true;
        foreach (Behavior behavior in sender.Behaviors)
        {
            if (behavior is ValidationBehavior validationBehavior)
                isValid &= validationBehavior.IsValid;       
        }
        model.Entry.IsValid = isValid;
    }
}