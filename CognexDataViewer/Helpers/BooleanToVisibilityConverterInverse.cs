using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CognexDataViewer.Helpers;

public class BooleanToVisibilityConverterInverse : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool && (bool)value)
        {
            return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility && (Visibility)value == Visibility.Visible)
        {
            return false;
        }
        return true;
    }
}
