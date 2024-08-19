using System.Globalization;
using System.Windows.Data;

namespace Photobox.UI;

public class DivideByThreeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double ret = default;
        if (value is double doubleValue)
        {
            ret = doubleValue / 3;
        }
        return ret;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return default!;
    }
}
