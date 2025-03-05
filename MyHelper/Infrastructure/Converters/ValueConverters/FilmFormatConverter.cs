using MyHelper.Models.Enums;
using System.Globalization;
using System.Windows.Data;

namespace MyHelper.Infrastructure.Converters.ValueConverters
{
    [ValueConversion(typeof(FilmFormat), typeof(string))]
    internal class FilmFormatConverter : Base.ValueConverter
    {
        private readonly string[] _formats = ["Полнометражный фильм", "Короткометражный фильм", "Сериал", "Мини-сериал", "Телевизионный фильм",
                                "Веб-сериал", "Реалити-шоу", "Ток-шоу", "Концерт", "Музыкльное видео"];
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is not int format ? string.Empty
            : format >= _formats.Length || format < 0 ? "Неизвестный формат!"
            : _formats[format];
    }
}
