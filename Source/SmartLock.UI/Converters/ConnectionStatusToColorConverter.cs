using System.Globalization;
using SmartLock.Client.NotificationHub;

namespace SmartLock.UI.Converters;

public class ConnectionStatusToColorConverter : IValueConverter
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
                return Colors.Red;
            case ConnectionStatus.Created:
            case ConnectionStatus.Connecting:
                return Colors.Yellow;
            case ConnectionStatus.Connected:
                return Colors.Green;
            case ConnectionStatus.Disconnected:
                return Colors.Red;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown connection status");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Color c)
        {
            return null;
        }

        if (Equals(c, Colors.Red))
            return ConnectionStatus.Disconnected;
        if (Equals(c, Colors.Yellow))
            return ConnectionStatus.Connecting;
        if (Equals(c, Colors.Green))
            return ConnectionStatus.Connected;
        return Colors.White;
    }
}