using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LiveAppsOverlay.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class SelectedProcessToBrushConverter : IValueConverter
    {
        static SelectedProcessToBrushConverter()
        {
            Instance = new SelectedProcessToBrushConverter();
        }

        public static SelectedProcessToBrushConverter Instance { get; private set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.LightBlue : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
