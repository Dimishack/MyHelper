using MyHelper.DAL.Entyties;
using System.Globalization;
using System.Text;

namespace MyHelper.Infrastructure.Converters.ValueConverters
{
    internal class FilmGenresConverter : Base.ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ICollection<FilmGenre> genres) return string.Empty;
            StringBuilder sb = new();
            foreach (var genre in genres)
                sb.Append($"{genre.Genre.Name}, ");
            return sb.Remove(sb.Length - 2, 2).ToString();
        }
    }
}
