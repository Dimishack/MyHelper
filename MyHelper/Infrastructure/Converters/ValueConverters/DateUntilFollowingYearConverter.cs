using System.Globalization;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters.ValueConverters
{
    class DateUntilFollowingYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => new DateTime(DateTime.Today.Year + 1, 12, 31);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
