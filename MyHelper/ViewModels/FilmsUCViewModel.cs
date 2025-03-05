using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Extensions;
using MyHelper.Interfaces;
using MyHelper.Models;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

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
        private readonly Filter[] _filterFormat = [new ("Все", true), new("Полнометражный фильм"), new("Короткометражный фильм"), new("Сериал"), new("Мини-сериал"),
                                                   new("Телевизионный фильм"), new("Веб-сериал"), new("Реалити-шоу"), new("Ток-шоу"), new("Концерт"), new("Музыкальное видео")];
        private readonly Filter[] _filterStatus = [new("Все", true), new("Запланирован"), new("Смотрю"), new("Посмотрен")];
        private readonly Dictionary<string, (Func<Film, object> func, bool ascending)> _sorts = new()
        {
            {"Сначала старые записи", (f => f.Id, true) },
            {"Сначала новые записи", (f => f.Id, false)},
            {"По названию (A -> Я)", (f => f.Name, true)},
            {"По названию (Я -> A)", (f => f.Name, false)},
            {"Сначала старые фильмы", (f => f.ReleaseYear, true)},
            {"Сначала новые фильмы", (f => f.ReleaseYear, false)},
            {"Сначала отрицательный рейтинг", (f => f.Raiting, true)},
            {"Сначала положительный рейтинг", (f => f.Raiting, false)},
        };

        private int _filmCount = 0;
        private int _filmCountWithFilter = 0;
        private int _selectedFilterFormat = -1;
        private int _selectedFilterStatus = -1;
        private bool _changePages = false;

        public ICollectionView FilmsCollectionView => CollectionViewSource.GetDefaultView(_films);
        public ICollectionView PagesCollectionView => CollectionViewSource.GetDefaultView(_pages);
        public IReadOnlyDictionary<string, (Func<Film, object> func, bool ascending)> Sorts => _sorts;
        public IEnumerable<Filter> FilterFormat => _filterFormat;
        public IEnumerable<Filter> FilterStatus => _filterStatus;

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
                else if (value == _pages[^1] && value < GetPageCount())
                {
                    _pages.Add(_pages[^1] + 1);
                    _pages.RemoveAt(0);
                }
                else if (value == 1 && _pages[0] != 1)
                {
                    _pages.ClearAndAddElements(Enumerable.Range(1, 10));
                    OnPropertyChanged(nameof(SelectedPage));
                }
                SetFilms();
            }
        }

        #endregion

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

        #region Commands...

        protected override void OnLoadedCommandExecuted(object? p)
        {
            _films.ClearAndAddElements(_itemsRepository.Items.Take(MAXCOUNT));
            _filmCount = _filmCountWithFilter = _itemsRepository.Items.Count();
            int countPage = GetPageCount();
            _pages.ClearAndAddElements(Enumerable.Range(1, countPage < 10 ? countPage : 10));
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

        #region FilterByFormatCommand - Команда - фильтровать по формату

        ///<summary>Команда - фильтровать по формату</summary>
        private ICommand? _filterByFormatCommand;

        ///<summary>Команда - фильтровать по формату</summary>
        public ICommand FilterByFormatCommand => _filterByFormatCommand
            ??= new LambdaCommand<string>(OnFilterByFormatCommandExecuted, CanFilterByFormatCommandExecute);

        private bool CanFilterByFormatCommandExecute(string p) => _filmCount > MAXCOUNT;

        ///<summary>Логика выполнения - фильтровать по формату</summary>
        private void OnFilterByFormatCommandExecuted(string p)
        {
            _selectedFilterFormat = Array.FindIndex(_filterFormat, f => f.Name == p) - 1;
            _changePages = true;
            if (_selectedPage > 1) SelectedPage = 1;
            else SetFilms();
        }

        #endregion

        #region FilterByStatusCommand - Команда - фильтровать по статусу

        ///<summary>Команда - фильтровать по статусу</summary>
        private ICommand? _filterByStatusCommand;

        ///<summary>Команда - фильтровать по статусу</summary>
        public ICommand FilterByStatusCommand => _filterByStatusCommand
            ??= new LambdaCommand<string>(OnFilterByStatusCommandExecuted, CanFilterByStatusCommandExecute);

        private bool CanFilterByStatusCommandExecute(string p) => _filmCount > MAXCOUNT;

        ///<summary>Логика выполнения - фильтровать по статусу</summary>
        private void OnFilterByStatusCommandExecuted(string p)
        {
            _selectedFilterStatus = Array.FindIndex(_filterStatus, f => f.Name == p) - 1;
            _changePages = true;
            if (_selectedPage > 1) SelectedPage = 1;
            else SetFilms();
        }

        #endregion

        #endregion
        private int GetPageCount() => _filmCountWithFilter / MAXCOUNT + (_filmCountWithFilter % MAXCOUNT == 0 ? 0 : 1);

        private void SetFilms()
        {
            var films = _itemsRepository.Items;
            if (_selectedFilterFormat > -1)
                films = films.Where(f => f.Format == _selectedFilterFormat);
            if (_selectedFilterStatus > -1)
                films = films.Where(f => f.Status == _selectedFilterStatus);
            if (_changePages)
            {
                _filmCountWithFilter = films.Count();
                int countPage = GetPageCount();
                _pages.ClearAndAddElements(Enumerable.Range(1, countPage < 10 ? countPage : 10));
                OnPropertyChanged(nameof(SelectedPage));
                _changePages = false;
            }
            _films.ClearAndAddElements(films
                .Sort(_selectedSort.Value.func, _selectedSort.Value.ascending)
                .Skip((_selectedPage - 1) * MAXCOUNT)
                .Take(MAXCOUNT));
        }

    }
}
