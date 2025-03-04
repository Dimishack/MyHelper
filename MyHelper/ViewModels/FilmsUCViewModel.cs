using MyHelper.DAL.Entyties;
using MyHelper.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace MyHelper.ViewModels
{
    internal sealed class FilmsUCViewModel(IRepository<Film> filmRepository,
                                           IRepository<Genre> genreRepository,
                                           IRepository<FilmGenre> filmGenreRepository) : MainFunctionsViewModel<Film>(filmRepository)
    {
        const int MAXCOUNT = 50;

        private readonly IRepository<Genre> _genreRepository = genreRepository;
        private readonly IRepository<FilmGenre> _filmGenreRepository = filmGenreRepository;
        private readonly ObservableCollection<Film> _films = [];
        private readonly ObservableCollection<int> _pages = [];

        private int _pageCount => _itemsRepository.Items.Count() / MAXCOUNT;

        public ICollectionView FilmsCollectionView => CollectionViewSource.GetDefaultView(_films);

        public ICollectionView PagesCollectionView => CollectionViewSource.GetDefaultView(_pages);

        #region SelectedPage : int - Выбранная страница

        ///<summary>Выбранная страница</summary>
        private int _selectedPage = 1;

        ///<summary>Выбранная страница</summary>
        public int SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (!Set(ref _selectedPage, value)) return;
                if (value == _pages[0] && value > 1)
                {
                    _pages.Insert(0, _pages[0] - 1);
                    _pages.RemoveAt(_pages.Count - 1);
                }
                else if (value == _pages[^1] && value < _pageCount)
                {
                    _pages.Add(_pages[^1] + 1);
                    _pages.RemoveAt(0);
                }
                SetFilms();
            }
        }

        #endregion

        public Dictionary<string, (Func<Film, object> func, bool ascending)> Sorts { get; } = new()
        {
            {"Сначала старые записи", (c => c.Id, true) },
            {"Сначала новые записи", (c => c.Id, false)},
            {"По названию (A -> Я)", (c => c.Name, true)},
            {"По названию (Я -> A)", (c => c.Name, false)},
            {"Сначала старые фильмы", (c => c.ReleaseYear, true)},
            {"Сначала новые фильмы", (c => c.ReleaseYear, false)},
            {"Сначала отрицательный рейтинг", (c => c.Raiting, true)},
            {"Сначала положительный рейтинг", (c => c.Raiting, false)}
        };

        #region SelectedSort : KeyValuePair<string, (Func<Film, object> func, bool ascending)> - Выбранная сортировка

        ///<summary>Выбранная сортировка</summary>
        private KeyValuePair<string, (Func<Film, object> func, bool ascending)> _selectedSort = KeyValuePair
            .Create<string, (Func<Film, object> func, bool ascending)>("Сначала старые записи", (c => c.Id, true));

        ///<summary>Выбранная сортировка</summary>
        public KeyValuePair<string, (Func<Film, object> func, bool ascending)> SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (!Set(ref _selectedSort, value)) return;

                if (_selectedPage > 1) SelectedPage = 1;
                else SetFilms();
            }
        }

        #endregion

        private void SetFilms()
        {
            var films = _selectedSort.Value.ascending
                ? _itemsRepository.Items.OrderBy(_selectedSort.Value.func).Skip((_selectedPage - 1) * MAXCOUNT).Take(MAXCOUNT)
                : _itemsRepository.Items.OrderByDescending(_selectedSort.Value.func).Skip((_selectedPage - 1) * MAXCOUNT).Take(MAXCOUNT);
            _films.ClearAndAddElements(films);
        }

        #region Commands...

        protected override void OnLoadedCommandExecuted(object? p)
        {
            foreach (var film in _itemsRepository.Items.Take(MAXCOUNT))
                _films.Add(film);
            int countPage = _pageCount;
            for (var i = 1; i <= countPage && i <= 10; i++)
                _pages.Add(i);
        }

        protected override Task OnAddElementCommandExecuted(object? p)
        {
            throw new NotImplementedException();
        }

        protected override Task OnDeleteElementCommandExecuted(object? p)
        {
            throw new NotImplementedException();
        }

        protected override Task OnEditElementCommandExecuted(object? p)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
