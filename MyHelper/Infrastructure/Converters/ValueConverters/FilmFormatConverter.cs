using MyHelper.Models.Enums;
using System.Globalization;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters.ValueConverters
{
    [ValueConversion(typeof(int), typeof(string))]
    internal class FilmFormatConverter : Base.ValueConverter
    {
        private readonly string[] _formats = Enum.GetNames(typeof(FilmFormat));
        private readonly string[] _formats_ru = ["Полнометражный фильм", "Короткометражный фильм", "Сериал", "Мини-сериал", "Телевизионный фильм",
                                "Веб-сериал", "Реалити-шоу", "Ток-шоу", "Концерт", "Музыкальное видео"];
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is not int format
            ? string.Empty
            : format >= _formats_ru.Length && format < _formats.Length
            ? _formats[format]
            : format >= _formats.Length || format < 0
            ? "Неизвестный формат!"
            : _formats_ru[format];
    }
}
