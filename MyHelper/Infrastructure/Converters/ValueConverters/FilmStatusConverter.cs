using MyHelper.Models.Enums;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters.ValueConverters
{
    [ValueConversion(typeof(int), typeof(string))]
    class FilmStatusConverter : Base.ValueConverter
    {
        private readonly string[] _statuses = new string[Enum.GetNames(typeof(FilmStatus)).Length];

        public FilmStatusConverter()
        {
            int index = 0;
            var fields = typeof(FilmStatus).GetFields();
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                    _statuses[index++] = attribute.Description;
            }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is not int status
            ? string.Empty
            : status >= _statuses.Length || status < 0
            ? "Неизвестный статус!"
            : _statuses[status];
    }
}
