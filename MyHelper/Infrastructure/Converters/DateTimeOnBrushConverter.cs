using MyHelper.Models.Challenges;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyHelper.Infrastructure.Converters
{
    [ValueConversion(typeof(DateTime), typeof(Brushes))]
    internal class DateTimeOnBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var checklist = (IList<Checklist>)value;
            var dateStart = checklist[0].Date;
            var dateEnd = checklist[^1].Date;
            if (dateStart > DateTime.Today)
                return Brushes.Coral;
            else if (dateEnd < DateTime.Today)
                return Brushes.LimeGreen;
            else
                return Brushes.Yellow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Данный метод не поддерживается");
        }
    }
}
