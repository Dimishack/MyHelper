using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters
{
    internal class ShowAdditionalRegularityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || values[0] is not string regularity || values[1] is not int additional) return null;
            if(regularity == "Кол-во дней в неделю")
                return additional;
            else if (regularity == "По дням недели")
            {
                StringBuilder result = new ();
                int number = additional;
                while(number > 0)
                {
                    int divide = 10;
                    var value = (number % divide) - 1;
                    result.Append((DayOfWeek)value + ", ");
                    number /= divide;
                }
                return result.ToString();
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
