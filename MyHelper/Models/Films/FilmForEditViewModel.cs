using MyHelper.DAL.Entyties;
using MyHelper.Models.Enums;
using System.ComponentModel;
using System.Reflection;

namespace MyHelper.Models.Films
{
    internal class FilmForEditViewModel
    {

        private readonly Film _film;
        private readonly string[] _formats = new string[Enum.GetNames(typeof(FilmFormat)).Length];
        private readonly string[] _statuses = new string[Enum.GetNames(typeof(FilmStatus)).Length];
        private readonly int[] _raitings = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        public IReadOnlyList<string> Formats => _formats;
        public IReadOnlyList<string> Statuses => _statuses;
        public IReadOnlyList<int> Raitings => _raitings;

        private readonly List<ObservableKeyValuePair<Tuple<int, string>, bool>> _genres = [];
        public IReadOnlyCollection<ObservableKeyValuePair<Tuple<int, string>, bool>> Genres => _genres;

        public int Id => _film.Id;
        public string Name
        {
            get => _film.Name;
            set => _film.Name = value;
        }

        public string Producer
        {
            get => _film.Producer;
            set => _film.Producer = value;
        }

        public int Format
        {
            get => _film.Format;
            set => _film.Format = value;
        }

        public int Status
        {
            get => _film.Status;
            set => _film.Status = value;
        }

        public int Raiting
        {
            get => _film.Raiting;
            set => _film.Raiting = value;
        }

        public int ReleaseYear
        {
            get => _film.ReleaseYear;
            set => _film.ReleaseYear = value;
        }

        public FilmForEditViewModel(Film film, in IEnumerable<Genre> genres)
        {
            _film = film;
            foreach (var genre in genres)
                _genres.Add(new(Tuple.Create(genre.Id, genre.Name), false));
            if (film.FilmGenres is not null)
                foreach (var filmGenre in film.FilmGenres)
                    _genres[filmGenre.GenreId - 1].Value = true;

            int index = 0;
            foreach (var field in typeof(FilmFormat).GetFields())
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                    _formats[index++] = attribute.Description;
            }
            index = 0;
            foreach (var field in typeof(FilmStatus).GetFields())
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                    _statuses[index++] = attribute.Description;
            }
        }

        public void CopyTo(Film film)
        {
            film.Id = _film.Id;
            film.Name = _film.Name;
            film.Producer = _film.Producer;
            film.Format = _film.Format;
            film.Status = _film.Status;
            film.Raiting = _film.Raiting;
            film.ReleaseYear = _film.ReleaseYear;
            var genres = _genres.Where(i => i.Value);
            if (film.FilmGenres is null)
            {
                film.FilmGenres = [];
                foreach (var genre in genres)
                {
                    film.FilmGenres.Add(new FilmGenre()
                    {
                        FilmId = _film.Id,
                        GenreId = genre.Key.Item1
                    });
                }
            }
            else
            {
                var test = film.FilmGenres.ToList();
                film.FilmGenres.Clear();
                foreach (var genre in genres)
                {
                    var ttt = test.FirstOrDefault(i => i.GenreId == genre.Key.Item1);
                    film.FilmGenres.Add(ttt is not null
                        ? ttt
                        : new FilmGenre()
                        {
                            FilmId = _film.Id,
                            GenreId = genre.Key.Item1
                        });
                }
            }
            if (_film.ViewingDate is null && _film.Status == 2)
                film.ViewingDate = DateTime.Now;
            else if (_film.ViewingDate is not null && _film.Status != 2)
                film.ViewingDate = null;
            else film.ViewingDate = _film.ViewingDate;
        }

        public void CopyFrom(Film film)
        {
            _film.Id = film.Id;
            _film.Name = film.Name;
            _film.Producer = film.Producer;
            _film.Status = film.Status;
            _film.Format = film.Format;
            _film.ViewingDate = film.ViewingDate;
            _film.ReleaseYear = film.ReleaseYear;
            _film.FilmGenres = film.FilmGenres;
            for (int i = 0; i < _genres.Count; i++)
                _genres[i].Value = false;
            if (film.FilmGenres is not null)
                foreach (var filmGenre in film.FilmGenres)
                    _genres[filmGenre.GenreId - 1].Value = true;
        }
    }
}
