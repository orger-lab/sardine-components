using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sardine.Devices.Hamamatsu.Camera.Views.WPF
{
    public class PixelSizeToThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is uint && values[1] is uint)
            {
                return new Thickness((uint)(System.Convert.ToDouble(values[0]) * 200 / 2048)+5,(uint)(System.Convert.ToDouble(values[1]) * 200 / 2048)+5,0,0);
            }
            return new Thickness(0, 0, 0, 0);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
