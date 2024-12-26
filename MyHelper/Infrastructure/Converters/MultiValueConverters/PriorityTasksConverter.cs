using System.Globalization;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters.MultiValueConverters
{
    internal class PriorityTasksConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string result = string.Empty;
            if(values.Length == 2 && values[0] is bool important && values[1] is bool prompt)
            {
                string importantStr = important ? "Важные" : "Неважные";
                string promprtStr = prompt ? "Срочные" : "Несрочные";
                result = $"Приоритет: {importantStr}/{promprtStr}";
            }
            return result;
        } 

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
