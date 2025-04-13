using System;
using System.Windows.Data;
using System.Windows;
using Windows.Win32.Foundation;

namespace LiveAppsOverlay.Converters
{
    [ValueConversion(typeof(nint), typeof(string))]
    public class HwndToStringConverter : IValueConverter
    {
        static HwndToStringConverter()
        {
            Instance = new HwndToStringConverter();
        }

        public static HwndToStringConverter Instance { get; private set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            return (nint)value == 0 ? "null" : ((nint)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
