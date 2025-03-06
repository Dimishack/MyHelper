using System.Globalization;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters.MultiValueConverters
{
    class FilterWithCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string name && values[1] is int count)
                return $"{name} ({count})";
            else return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
