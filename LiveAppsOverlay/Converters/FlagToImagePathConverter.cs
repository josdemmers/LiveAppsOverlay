using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LiveAppsOverlay.Converters
{
    public class FlagToImagePathConverter : IValueConverter
    {
        static FlagToImagePathConverter()
        {
            Instance = new FlagToImagePathConverter();
        }

        public static FlagToImagePathConverter Instance { get; private set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string flag = (string)value;
                flag = flag.Replace("-", string.Empty);

                var uri = new Uri($"pack://application:,,,/Images/Flags/{flag}.png");
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = uri;
                bitmap.EndInit();

                return bitmap;
            }
            catch (Exception)
            {
                // TODO: Add logging converter
                //var eventAggregator = (IEventAggregator)Prism.Ioc.ContainerLocator.Container.Resolve(typeof(IEventAggregator));
                //eventAggregator.GetEvent<ExceptionOccurredEvent>().Publish(new ExceptionOccurredEventParams
                //{
                //    Message = $"File not found: ./Images/Flags/{(string)value}.png"
                //});
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}