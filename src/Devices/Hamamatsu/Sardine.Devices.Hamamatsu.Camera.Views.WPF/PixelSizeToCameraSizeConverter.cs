using System;
using System.Globalization;
using System.Windows.Data;

namespace Sardine.Devices.Hamamatsu.Camera.Views.WPF
{
    public class PixelSizeToCameraSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is uint)
            {
                return (uint)(System.Convert.ToDouble(value) * 200 / 2048);
            }

            return 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
