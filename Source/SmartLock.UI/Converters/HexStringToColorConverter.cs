using System.Globalization;

namespace SmartLock.UI.Converters;

public class HexStringToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // check if value is a string and if it is a valid argb hex string
        if (value is string hexString && hexString.Length == 9 && hexString.StartsWith("#"))
        {
            // convert hex string to color
            return Color.FromArgb(hexString);
        }
        else
        {
            throw new ArgumentException("Value must be a valid ARGB hex string");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // check if value is a color
        if (value is Color color)
        {
            // convert color to hex string
            return color.ToArgbHex();
        }
        else
        {
            throw new ArgumentException("Value must be a color");
        }
    }
}