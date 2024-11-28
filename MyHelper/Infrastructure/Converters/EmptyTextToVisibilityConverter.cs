using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters
{
    class EmptyTextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string text && !string.IsNullOrWhiteSpace(text)? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
