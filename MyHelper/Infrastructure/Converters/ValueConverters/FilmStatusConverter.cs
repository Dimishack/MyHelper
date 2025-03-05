using System.Globalization;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters.ValueConverters
{
    [ValueConversion(typeof(int), typeof(string))]
    class FilmStatusConverter : Base.ValueConverter
    {
        private readonly string[] _statuses = ["Запланирован", "Смотрю", "Посмотрен"];
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is not int status ? string.Empty
            : status >= _statuses.Length || status < 0 ? "Неизвестный статус!"
            : _statuses[status];
    }
}
