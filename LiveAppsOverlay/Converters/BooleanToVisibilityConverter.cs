using System;
using System.Windows.Data;
using System.Windows;

namespace LiveAppsOverlay.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        static BooleanToVisibilityConverter()
        {
            Instance = new BooleanToVisibilityConverter();
        }

        public static BooleanToVisibilityConverter Instance { get; private set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            return value != null && ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
