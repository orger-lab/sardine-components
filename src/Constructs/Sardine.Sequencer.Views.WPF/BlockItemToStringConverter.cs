using System.Globalization;
using System.Windows.Data;

namespace Sardine.Sequencer.Views.WPF
{
    public class BlockItemToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IPatternBlockProvider blockProvider)
            {
                return blockProvider.BlockName;
            }
            throw new Exception();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception();
        }
    }
}
    
