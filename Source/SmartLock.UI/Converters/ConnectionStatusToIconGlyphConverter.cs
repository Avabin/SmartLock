using System.Globalization;
using SmartLock.Client.NotificationHub;
#pragma warning disable CS8603

namespace SmartLock.UI.Converters;

public class ConnectionStatusToIconGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ConnectionStatus v)
        {
            return null;
        }
        switch (v)
        {
            case ConnectionStatus.Unknown:
                return MaterialIcons.CloudQuestionOutline;
            case ConnectionStatus.Created:
                return MaterialIcons.CloudRefreshOutline;
            case ConnectionStatus.Connecting:
                return MaterialIcons.CloudRefreshOutline;
            case ConnectionStatus.Connected:
                return MaterialIcons.CloudCheckOutline;
            case ConnectionStatus.Disconnected:
                return MaterialIcons.CloudCancelOutline;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown connection status");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string v)
        {
            return null;
        }
        
        switch (v)
        {
            case MaterialIcons.CloudQuestionOutline:
                return ConnectionStatus.Unknown;
            case MaterialIcons.CloudRefreshOutline:
                return ConnectionStatus.Connecting;
            case MaterialIcons.CloudCheckOutline:
                return ConnectionStatus.Connected;
            case MaterialIcons.CloudCancelOutline:
                return ConnectionStatus.Disconnected;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown connection status");
        }
    }
}