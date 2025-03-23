using MyHelper.Models.Enums;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters.ValueConverters
{
    [ValueConversion(typeof(int), typeof(string))]
    internal class FilmFormatConverter : Base.ValueConverter
    {
        private readonly string[] _formats = new string[Enum.GetNames(typeof(FilmFormat)).Length];

        public FilmFormatConverter()
        {
            int index = 0;
            var fields = typeof(FilmFormat).GetFields();
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                    _formats[index++] = attribute.Description;
            }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is not int format
            ? string.Empty
            : format >= _formats.Length || format < 0
            ? "Неизвестный формат!"
            : _formats[format];
    }
}
