using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LiveAppsOverlay.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BooleanToRedGreenBrushConverter : IValueConverter
    {
        static BooleanToRedGreenBrushConverter()
        {
            Instance = new BooleanToRedGreenBrushConverter();
        }

        public static BooleanToRedGreenBrushConverter Instance { get; private set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Green : Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
