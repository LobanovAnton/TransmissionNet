using System.Globalization;
using TransmissionNet.Extensions;
using TransmissionNet.TorrentProviders;

namespace TransmissionNet.Converters;

file static class ConverterHelper
{
    private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
    
    public static string SizeInUnits(double bytes, int unitSize)
    {
        int unit = 0;

        double size = bytes;
        while(size >= unitSize)
        {
            size /= unitSize;
            ++unit;
        }
        
        return $"{size:0.##} {Units[unit]}";
    }   
}

public class DecimalToUnitConverter: IMultiValueConverter
{
    public static readonly DecimalToUnitConverter Instance = new();
    
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        int unitSize = (int)values[1];
        if (unitSize <= 0) return 0;

        switch (values[0])
        {
            case double d:
                return ConverterHelper.SizeInUnits(d, unitSize);
            case long l:
                return ConverterHelper.SizeInUnits(l, unitSize);
            case int i:
                return ConverterHelper.SizeInUnits(i, unitSize);
            default:
                return ConverterHelper.SizeInUnits(0, unitSize);
        }   
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
    }
}

public class DoubleToStringConverter: IValueConverter
{
    public static readonly DoubleToStringConverter Instance = new();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double p = (double)(value ?? 0);
        return $"{p:0.##}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}

public class DoubleToStringPercentConverter: IValueConverter
{
    public static readonly DoubleToStringPercentConverter Instance = new();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double p = (double)(value ?? 0) * 100.0;
        return $"{p:0.##}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}

public class StatusToGlyphConverter: IValueConverter
{
    public static readonly StatusToGlyphConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        TorrentModel.State state = (TorrentModel.State)(value ?? TorrentModel.State.Stopped);
        switch (state)
        {
            case TorrentModel.State.WaitVerify:
            case TorrentModel.State.WaitDownload:
            case TorrentModel.State.WaitingSeed:
            case TorrentModel.State.Verifying:
            case TorrentModel.State.Downloading:
            case TorrentModel.State.Seeding:
                return "\uEDA7";
            case TorrentModel.State.Stopped:
            default:
                return "\uEDE0";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}

public class SortModeToGlyphConverter: IValueConverter
{
    public static readonly SortModeToGlyphConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        SortMode mode = (SortMode)(value ?? SortMode.Descending);
        switch (mode)
        {
            case SortMode.Ascending:
                return "\uEB0F";
            case SortMode.Descending:
            default:
                return "\uEB03";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}

public class StatusToColorConverter: IValueConverter
{
    public static readonly StatusToColorConverter Instance = new();

    private readonly object _primary = Colors.Black;
    private readonly object _primaryDark = Colors.White;

    public StatusToColorConverter()
    {
        object? result = null;
        Application.Current?.Resources.TryGetValue("Primary", out result);
        if (result is Color primary)
            _primary = primary;
        Application.Current?.Resources.TryGetValue("PrimaryDark", out result);
        if (result is Color primaryDark)
            _primaryDark = primaryDark;
    }
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        TorrentModel.State state = (TorrentModel.State)(value ?? TorrentModel.State.Stopped);
        switch (state)
        {
            case TorrentModel.State.WaitVerify:
            case TorrentModel.State.WaitDownload:
            case TorrentModel.State.WaitingSeed:
            case TorrentModel.State.Verifying:
            case TorrentModel.State.Downloading:
            case TorrentModel.State.Seeding:
                return Colors.Peru;
            case TorrentModel.State.Stopped:
            default:
            {
                switch (Application.Current?.RequestedTheme)
                {
                    case AppTheme.Dark:
                        return _primaryDark;
                        default:
                        return _primary;
                }
            }
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}

public class StatusToStringConverter: IValueConverter
{
    public static readonly StatusToStringConverter Instance = new();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return "Unknown";

        TorrentModel.State state = (TorrentModel.State)value;
        switch (state)
        {
            case TorrentModel.State.Stopped:
                return "Stopped";
            case TorrentModel.State.WaitVerify:
            case TorrentModel.State.WaitDownload:
            case TorrentModel.State.WaitingSeed:
                return "Waiting";
            case TorrentModel.State.Verifying:
                return "Verifying";
            case TorrentModel.State.Downloading:
                return "Downloading";
            case TorrentModel.State.Seeding:
                return "Seeding";
            default:
                return "Unknown";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}

public class SecondsToStringTimeConverter: IValueConverter
{
    public static readonly SecondsToStringTimeConverter Instance = new();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int seconds = (int)(value ?? 0);
        if (seconds == int.MaxValue)
            return "unknown";
        TimeSpan timeSpan = new TimeSpan(0, 0, seconds);
        if (timeSpan.Days > 0)
            return timeSpan.ToString(@"d"" day(s) ""hh\:mm\:ss");
        if (timeSpan.Hours > 0)
            return timeSpan.ToString(@"hh\:mm\:ss");
        return timeSpan.ToString(@"mm\:ss");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}

public class FullPathToName: IValueConverter
{
    public static readonly FullPathToName Instance = new();
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string[] paths)
        {
            string[] pathNames = new string[paths.Length];
            for (int i = 0; i < paths.Length; i++)
                pathNames[i] = new DirectoryInfo(paths[i]).Name;
            return pathNames;
        }
        return Array.Empty<string>();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}
