using System.Globalization;
using SmartLock.UI.Features.Buildings.ViewModels;

namespace SmartLock.UI.Converters;

public class LockViewModelToFontImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LockViewModel lockViewModel)
        {
            return lockViewModel.IsLocked ? MaterialIcons.LockOutline : MaterialIcons.LockOpenOutline;
        }

        return MaterialIcons.LockOutline;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}