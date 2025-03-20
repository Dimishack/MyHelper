using MyHelper.DAL.Entyties;
using MyHelper.Models.Enums;

namespace MyHelper.Models.Films
{
    internal class FilmForEditViewModel
    {

        private readonly Film _film;
        private readonly string[] _formats = Enum.GetNames(typeof(FilmFormat));
        private readonly string[] _formats_ru = ["Полнометражный фильм", "Короткометражный фильм", "Сериал", "Мини-сериал", "Телевизионный фильм",
                                "Веб-сериал", "Реалити-шоу", "Ток-шоу", "Концерт", "Музыкальное видео"];

        public IReadOnlyList<string> Formats
        {
            get
            {
                List<string> formats = new(Enumerable.Range(0, _formats_ru.Length).Select(i => _formats_ru[i]));
                if (formats.Count < _formats.Length)
                {
                    for (int i = formats.Count - 1; i < _formats.Length; i++)
                        formats.Add(_formats[i]);
                }
                return formats;
            }
        }

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
                _genres.Add(new(Tuple.Create(genre.Id,genre.Name), false));
            if (film.FilmGenres is not null)
                foreach (var filmGenre in film.FilmGenres)
                    _genres[filmGenre.GenreId - 1].Value = true;
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
            if(film.FilmGenres is not null)
                foreach (var filmGenre in film.FilmGenres)
                    _genres[filmGenre.GenreId - 1].Value = true;
        }

    }
}
