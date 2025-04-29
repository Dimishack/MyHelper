using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Attributes;
using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Extensions;
using MyHelper.Interfaces;
using MyHelper.Models;
using MyHelper.Models.Films;
using MyHelper.Models.Structs;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal sealed class FilmsUCViewModel(IRepository<Film> filmRepository,
                                           IRepository<Genre> genreRepository,
                                           IRepository<FilmGenre> filmGenreRepository,
                                           IPropertyDependency propertyDependency) : CrudViewModelBase<Film>(filmRepository, propertyDependency)
    {
        private record FilmWithGenreDto
        {
            public Film Film { get; init; } = default!;
            public int GenreId { get; init; }
        }

        private record FilterCountDto
        {
            public int Key { get; init; }
            public int Count { get; init; }
        }

        const int MAXCOUNT = 50;
        private Search _currentSearch = new(string.Empty, string.Empty);
        private event EventHandler? SelectedFilmsChanged;
        private readonly IRepository<Genre> _genreRepository = genreRepository;
        private readonly IRepository<FilmGenre> _filmGenreRepository = filmGenreRepository;
        private int _filmCount = 0;
        private int _selectedFilterFormat = -1;
        private int _selectedFilterStatus = -1;
        private int _selectedFilterGenre = 0;
        private bool _inFirstPage = false;

        #region Properties...

        [ConnectedProperties(nameof(IsEditFilm))]
        public override bool IsElementEnabled => base.IsElementEnabled;

        #region CountPages : int - Количество страниц

        ///<summary>Количество страниц</summary>
        private int _countPages;

        ///<summary>Количество страниц</summary>
        public int CountPages { get => _countPages; set => Set(ref _countPages, value); }

        #endregion

        #region Films : ObservableCollection<Film> - Список фильмов

        /// <summary> Список фильмов </summary>
        private readonly ObservableCollection<Film> _films = [];
        /// <summary> Список фильмов </summary>
        public ICollectionView Films => CollectionViewSource.GetDefaultView(_films);

        #endregion

        #region Pages : ObservableCollection<int> - Список страниц

        /// <summary> Список страниц </summary>
        private readonly ObservableCollection<int> _pages = [];
        /// <summary> Список страниц </summary>
        public IReadOnlyCollection<int> Pages => _pages;

        #endregion

        #region Sorts : Dictionary<string, (Expreccion<Func<Film, object>> func, bool ascending)> - Список с сортировкой

        /// <summary> Список с сортировкой </summary>
        private readonly Dictionary<string, (Expression<Func<Film, object>> func, bool ascending)> _sorts = new()
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
        /// <summary> Список с сортировкой </summary>
        public IReadOnlyDictionary<string, (Expression<Func<Film, object>> func, bool ascending)> Sorts => _sorts;

        #endregion

        #region FilterFormat : Filter[] - Список с фильтром по формату

        /// <summary> Список с фильтром по формату </summary>
        private readonly Filter[] _filterFormat = [new ("Все", true), new("Полнометражный фильм"), new("Короткометражный фильм"), new("Сериал"), new("Мини-сериал"),
                                                   new("Телевизионный фильм"), new("Веб-сериал"), new("Реалити-шоу"), new("Ток-шоу"), new("Концерт"), new("Музыкальное видео")];
        /// <summary> Список с фильтром по формату </summary>
        public IReadOnlyList<Filter> FilterFormat => _filterFormat;

        #endregion

        #region FilterStatus : Filter[] - Список с фильтром по статусу

        /// <summary> Список с фильтром по статусу </summary>
        private readonly Filter[] _filterStatus = [new("Все", true), new("Запланирован"), new("Смотрю"), new("Посмотрен")];

        /// <summary> Список с фильтром по статусу </summary>
        public IReadOnlyList<Filter> FilterStatus => _filterStatus;

        #endregion

        #region ArrayProperties : string[] - Список свойств для поиска

        /// <summary> Список свойств для поиска </summary>
        private readonly string[] _arrayProperties = ["Автор", "Название фильма"];
        /// <summary> Список свойств для поиска </summary>
        public IReadOnlyList<string> ArrayProperties => _arrayProperties;

        #endregion

        #region SelectedProperty : string? - выбранное свойство для поиска

        ///<summary>выбранное свойство для поиска</summary>
        private string? _selectedProperty;

        ///<summary>выбранное свойство для поиска</summary>
        public string? SelectedProperty { get => _selectedProperty; set => Set(ref _selectedProperty, value); }

        #endregion

        #region FilterGenre : ObservableCollection<Filter> - Список с фильтром по жанру

        /// <summary> Список с фильтром по жанру </summary>
        private readonly ObservableCollection<Filter> _filterGenre =
        [
            new("Все", true),
        ];
        /// <summary> Список с фильтром по жанру </summary>
        public IReadOnlyCollection<Filter> FilterGenre => _filterGenre;

        #endregion

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
                else if (value == _pages[^1] && value < _countPages)
                {
                    _pages.Add(_pages[^1] + 1);
                    _pages.RemoveAt(0);
                }
                if (!_inFirstPage)
                    OnSelectedFilmsChanged();
            }
        }

        #endregion

        #region SelectedSort : KeyValuePair<string, (Func<Film, object> func, bool ascending)> - Выбранная сортировка

        ///<summary>Выбранная сортировка</summary>
        private KeyValuePair<string, (Expression<Func<Film, object>> func, bool ascending)> _selectedSort = KeyValuePair
            .Create<string, (Expression<Func<Film, object>> func, bool ascending)>("Сначала старые записи", (c => c.Id, true));

        ///<summary>Выбранная сортировка</summary>
        public KeyValuePair<string, (Expression<Func<Film, object>> func, bool ascending)> SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (!Set(ref _selectedSort, value)) return;
                OnSelectedFilmsChanged();
            }
        }

        #endregion

        #region SelectedFilm : Film - Выбранный фильм

        ///<summary>Выбранный фильм</summary>
        private Film? _selectedFilm;

        [ConnectedProperties(nameof(IsEditFilm))]
        ///<summary>Выбранный фильм</summary>
        public Film? SelectedFilm
        {
            get => _selectedFilm;
            set
            {
                if (!Set(ref _selectedFilm, value)) return;

                Stopwatch sw = Stopwatch.StartNew();
                OnConnectedPropertyChanged();
                sw.Stop();
            }
        }

        #endregion

        #region FieldSearch : string - Поле поиска

        ///<summary>Поле поиска</summary>
        private string _fieldSearch = string.Empty;

        ///<summary>Поле поиска</summary>
        public string FieldSearch { get => _fieldSearch; set => Set(ref _fieldSearch, value); }

        #endregion

        #region FilmForEdit : FilmForEditViewModel - свойство для редактирования фильма

        ///<summary>свойство для редактирования фильма</summary>
        private FilmForEditViewModel _filmForEdit = new(new Film(), genreRepository.Items);

        ///<summary>свойство для редактирования фильма</summary>
        public FilmForEditViewModel FilmForEdit { get => _filmForEdit; set => Set(ref _filmForEdit, value); }

        #endregion

        #region IsEditFilm : bool - редактировать фильм

        ///<summary>редактировать фильм</summary>
        public bool IsEditFilm => IsElementEnabled && _selectedFilm is not null;

        #endregion

        #endregion

        #region Commands...

        protected override void OnLoadedCommandExecuted(object? p) { }

        #region LoadedAsyncCommand - Команда - асихронная загрузка окна

        ///<summary>Команда - асихронная загрузка окна</summary>
        private ICommand? _loadedAsyncCommand;

        ///<summary>Команда - асихронная загрузка окна</summary>
        public ICommand LoadedAsyncCommand => _loadedAsyncCommand
            ??= new LambdaCommandAsync(OnLoadedAsyncCommandExecuted, CanLoadedAsyncCommandExecute);

        ///<summary>Проверка возможности выполнения - асихронная загрузка окна</summary>
        private bool CanLoadedAsyncCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - асихронная загрузка окна</summary>
        private async Task OnLoadedAsyncCommandExecuted(object? p)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var test = await _genreRepository.Items.AsNoTracking().Select(j => new { j.Name, j.FilmGenres.Count }).ToListAsync();
            foreach (var genre in test)
                _filterGenre.Add(new(genre.Name, count: genre.Count));
            SelectedFilmsChanged += FilmsUCViewModel_SelectedFilmsChanged;
            _inFirstPage = true;
            await RequestAsync(true, false, true, true);
            _filterGenre[0].Count = _filmCount;
            sw.Stop();
            Debug.WriteLine($"Время загрузки фильмов (в мс): {sw.Elapsed.TotalMilliseconds}");
        }

        #endregion

        #region override AddElementCommand - Добавить фильм

        protected override void OnBeforeAddElement()
        {
            FilmForEdit.CopyFrom(new Film());
            OnPropertyChanged(nameof(FilmForEdit));
        }

        protected override bool CanAddElementCommandExecute(object? p) =>
            !string.IsNullOrWhiteSpace(_filmForEdit.Name)
            && !string.IsNullOrWhiteSpace(_filmForEdit.Producer)
            && _filmForEdit.ReleaseYear >= 1895
            && _filmForEdit.Genres.FirstOrDefault(i => i.Value) is not null;

        protected override async Task OnAddElementCommandExecuted(object? p)
        {
            Film newFilm = new();
            _filmForEdit.CopyTo(newFilm);
            await ItemsRepository.AddAsync(newFilm);
            foreach (var filmGenre in newFilm.FilmGenres)
                await _filmGenreRepository.AddAsync(filmGenre);

            _filterFormat[0].Count++;
            _filterFormat[newFilm.Format + 1].Count++;

            _filterStatus[0].Count++;
            _filterStatus[newFilm.Status + 1].Count++;

            _filterGenre[0].Count++;
            foreach (var filmGenreId in newFilm.FilmGenres.Select(i => i.GenreId))
                _filterGenre[filmGenreId].Count++;

            await RequestAsync(true, false, false, false);
            if (_selectedPage == _pages[^1] && _countPages - _pages[^1] == 1)
            {
                _pages.Add(_countPages);
                if (_countPages > 10)
                    _pages.RemoveAt(0);
            }

            IsAddingElement = false;
        }

        #endregion

        #region override EditElementCommand - Редактировать фильм

        protected override void OnBeforeEditElement()
        {
            if (_selectedFilm is not null)
            {
                FilmForEdit.CopyFrom(_selectedFilm);
                OnPropertyChanged(nameof(FilmForEdit));
            }
        }

        protected override bool CanEditElementCommandExecute(object? p) =>
            !string.IsNullOrWhiteSpace(_filmForEdit.Name)
            && !string.IsNullOrWhiteSpace(_filmForEdit.Producer)
            && _filmForEdit.ReleaseYear >= 1895
            && _filmForEdit.Genres.FirstOrDefault(i => i.Value) is not null
            && (string.Compare(_selectedFilm.Name, _filmForEdit.Name) != 0
            || string.Compare(_selectedFilm.Producer, _filmForEdit.Producer) != 0
            || _selectedFilm.Format != _filmForEdit.Format
            || _selectedFilm.Status != _filmForEdit.Status
            || _selectedFilm.Raiting != _filmForEdit.Raiting
            || !_filmForEdit.Genres.Where(i => i.Value).Select(i => i.Key.Item1).SequenceEqual(_selectedFilm.FilmGenres.Select(i => i.GenreId)))
            ;

        protected override async Task OnEditElementCommandExecuted(object? p)
        {
            static bool ChangeCountInFilters(Filter[] array, int oldFilter, int newFilter)
            {
                bool result = newFilter != oldFilter;
                if (result)
                {
                    array[newFilter + 1].Count++;
                    array[oldFilter + 1].Count--;
                }
                return result;
            }

            int oldFormat = _selectedFilm.Format;
            int newFormat = _filmForEdit.Format;
            int oldStatus = _selectedFilm.Status;
            int newStatus = _filmForEdit.Status;
            var genreIds = _selectedFilm.FilmGenres.Select(i => new { i.Id, i.GenreId }).ToList();

            FilmForEdit.CopyTo(SelectedFilm);

            var genresOnDelete = genreIds.ExceptBy(_selectedFilm.FilmGenres.Select(j => j.GenreId), i => i.GenreId).ToList();
            var genresOnAdd = _selectedFilm.FilmGenres.ExceptBy(genreIds.Select(j => j.GenreId), i => i.GenreId).ToList();

            if (_currentSearch && !(_currentSearch.Property.Contains("Автор") && _selectedFilm.Producer.Contains(_currentSearch.Value)
                || _currentSearch.Property.Contains("Название фильма") && _selectedFilm.Name.Contains(_currentSearch.Value)))
            {
                _filterFormat[0].Count--;
                _filterFormat[oldFormat + 1].Count--;

                _filterStatus[0].Count--;
                _filterStatus[oldStatus + 1].Count--;

                _filterGenre[0].Count--;
                foreach (var genreId in genreIds)
                    _filterGenre[genreId.GenreId].Count--;
            }
            else
            {
                ChangeCountInFilters(_filterFormat, oldFormat, newFormat);
                ChangeCountInFilters(_filterStatus, oldStatus, newStatus);
                foreach (var genreOnDelete in genresOnDelete)
                    _filterGenre[genreOnDelete.GenreId].Count--;
                foreach (var genreOnAdd in genresOnAdd)
                    _filterGenre[genreOnAdd.GenreId].Count++;
            }

            _filmGenreRepository.AutoSaveChanges = false;
            foreach (var genreOnDelete in genresOnDelete)
                await _filmGenreRepository.RemoveAsync(genreOnDelete.Id);
            foreach (var genreOnAdd in genresOnAdd)
                await _filmGenreRepository.AddAsync(genreOnAdd);
            await _filmGenreRepository.SaveChangedAsync();
            _filmGenreRepository.AutoSaveChanges = true;

            if (_currentSearch)
            {
                var film = await ItemsRepository.GetAsync(_selectedFilm.Id);
                FilmForEdit.CopyTo(film);
                await ItemsRepository.UpdateAsync(film);
                await RequestAsync(true, false, false, false);

            }
            else
                await ItemsRepository.UpdateAsync(_selectedFilm);

            Films.Refresh();
            IsEditingElement = false;
        }

        #endregion

        #region override DeleteElementCommand - Удалить фильм

        protected override bool CanDeleteElementCommandExecute(object? p) => 
            _selectedFilm is not null
            && IsElementEnabled;

        protected override async Task OnDeleteElementCommandExecuted(object? p)
        {

            _filterFormat[0].Count--;
            _filterFormat[_selectedFilm.Format + 1].Count--;

            _filterStatus[0].Count--;
            _filterStatus[_selectedFilm.Status + 1].Count--;

            _filterGenre[0].Count--;
            foreach (var filmGenreId in _selectedFilm.FilmGenres.Select(i => i.GenreId))
                _filterGenre[filmGenreId].Count--;
            await ItemsRepository.RemoveAsync(_selectedFilm.Id);
            await RequestAsync(true, false, false, false);

            if (_countPages < _pages[^1])
            {
                _pages.RemoveAt(_pages.Count - 1);
                if (_pages[0] > 1)
                    _pages.Insert(0, _pages[0] - 1);
            }
            if (_selectedPage - _countPages == 1 && _selectedPage > 1)
                SelectedPage--;
        }

        #endregion

        #region FilterByFormatCommand - Команда - фильтровать по формату

        ///<summary>Команда - фильтровать по формату</summary>
        private ICommand? _filterByFormatCommand;

        ///<summary>Команда - фильтровать по формату</summary>
        public ICommand FilterByFormatCommand => _filterByFormatCommand
            ??= new LambdaCommandAsync<string>(OnFilterByFormatCommandExecuted);

        ///<summary>Логика выполнения - фильтровать по формату</summary>
        private async Task OnFilterByFormatCommandExecuted(string p)
        {
            _selectedFilterFormat = Array.FindIndex(_filterFormat, f => f.Name == p) - 1;
            _inFirstPage = true;
            await RequestAsync(true, true, false, true);
        }

        #endregion

        #region FilterByStatusCommand - Команда - фильтровать по статусу

        ///<summary>Команда - фильтровать по статусу</summary>
        private ICommand? _filterByStatusCommand;

        ///<summary>Команда - фильтровать по статусу</summary>
        public ICommand FilterByStatusCommand => _filterByStatusCommand
            ??= new LambdaCommandAsync<string>(OnFilterByStatusCommandExecuted);

        ///<summary>Логика выполнения - фильтровать по статусу</summary>
        private async Task OnFilterByStatusCommandExecuted(string p)
        {
            _selectedFilterStatus = Array.FindIndex(_filterStatus, f => f.Name == p) - 1;
            _inFirstPage = true;
            await RequestAsync(true, true, true, false);
        }

        #endregion

        #region FilterByGenreAsyncCommand - Команда - фильтровать по жанру

        ///<summary>Команда - фильтровать по жанру</summary>
        private ICommand? _filterByGenreAsyncCommand;

        ///<summary>Команда - фильтровать по жанру</summary>
        public ICommand FilterByGenreAsyncCommand => _filterByGenreAsyncCommand
            ??= new LambdaCommandAsync<string>(OnFilterByGenreAsyncCommandExecuted);

        ///<summary>Логика выполнения - фильтровать по жанру</summary>
        private async Task OnFilterByGenreAsyncCommandExecuted(string p)
        {
            _selectedFilterGenre = Array.FindIndex(_filterGenre.ToArray(), f => f.Name.Equals(p));
            _inFirstPage = true;
            await RequestAsync(true, false, true, true);
        }

        #endregion

        #region SearchCommand - Команда - поиск

        ///<summary>Команда - поиск</summary>
        private ICommand? _searchCommand;

        ///<summary>Команда - поиск</summary>
        public ICommand SearchCommand => _searchCommand
            ??= new LambdaCommandAsync(OnSearchCommandExecuted, CanSearchCommandExecute);

        ///<summary>Проверка возможности выполнения - поиск</summary>
        private bool CanSearchCommandExecute(object? p) =>
            !string.IsNullOrWhiteSpace(_fieldSearch)
            && !string.IsNullOrEmpty(_selectedProperty)
            && (string.Compare(_currentSearch.Value, _fieldSearch) != 0
            || string.Compare(_currentSearch.Property, _selectedProperty) != 0)
            ;

        ///<summary>Логика выполнения - поиск</summary>
        private async Task OnSearchCommandExecuted(object? p)
        {
            _currentSearch.Set(true, _fieldSearch, _selectedProperty);
            _inFirstPage = true;
            await RequestAsync(true, true, true, true);
        }

        #endregion

        #region CancelSearchCommand - Команда - отменить поиск

        ///<summary>Команда - отменить поиск</summary>
        private ICommand? _cancelSearchCommand;

        ///<summary>Команда - отменить поиск</summary>
        public ICommand CancelSearchCommand => _cancelSearchCommand
            ??= new LambdaCommandAsync(OnCancelSearchCommandExecuted, CanCancelSearchCommandExecute);

        ///<summary>Проверка возможности выполнения - отменить поиск</summary>
        private bool CanCancelSearchCommandExecute(object? p) => _currentSearch;

        ///<summary>Логика выполнения - отменить поиск</summary>
        private async Task OnCancelSearchCommandExecuted(object? p)
        {
            _currentSearch.Set(false, string.Empty, string.Empty);
            SelectedProperty = null;
            FieldSearch = string.Empty;
            _inFirstPage = true;
            await RequestAsync(true, true, true, true);
        }

        #endregion

        #endregion

        #region Events...

        private async void FilmsUCViewModel_SelectedFilmsChanged(object? sender, EventArgs e) => await RequestAsync(false, false, false, false);

        #endregion

        #region Methods...

        #region override Dispose

        protected override void Dispose(bool disposing)
        {
            if (!Disposed)
            {

                SelectedFilmsChanged -= FilmsUCViewModel_SelectedFilmsChanged;
            }
        }

        #endregion

        private void OnSelectedFilmsChanged()
            => SelectedFilmsChanged?.Invoke(this, EventArgs.Empty);

        private int GetPageCount()
        {
            int result = _filmCount / MAXCOUNT + (_filmCount % MAXCOUNT == 0 ? 0 : 1);
            return result == 0 ? 1 : result;
        }

        private IQueryable<FilmWithGenreDto> GetFilteredFilms(IQueryable<Film> films,
            bool filterGenre,
            bool filterFormat,
            bool filterStatus)
        {
            bool applyFormatFilter = _selectedFilterFormat != -1;
            bool applyStatusFilter = _selectedFilterStatus != -1;
            bool applyGenreFilter = _selectedFilterGenre != 0;

            var result = films
                .Join(_filmGenreRepository.Items, film => film.Id, f => f.FilmId, (film, f) => new FilmWithGenreDto { Film = film, GenreId = f.GenreId });

            if (filterGenre && applyGenreFilter)
                result = result.Where(i => i.GenreId == _selectedFilterGenre);
            if (filterFormat && applyFormatFilter)
                result = result.Where(i => i.Film.Format == _selectedFilterFormat);
            if (filterStatus && applyStatusFilter)
                result = result.Where(i => i.Film.Status == _selectedFilterStatus);

            return result;
        }

        private async Task<List<FilterCountDto>> GetFilteredCountsAsync(IQueryable<Film> films,
            bool filterFormat,
            bool filterStatus,
            Expression<Func<Film, int>> groupBy)
        {
            return await GetFilteredFilms(films, true, filterFormat, filterStatus)
                    .Select(i => i.Film)
                    .Distinct()
                    .GroupBy(groupBy)
                    .Select(g => new FilterCountDto { Key = g.Key, Count = g.Count() })
                    .ToListAsync();
        }

        private async Task RequestAsync(bool changeCountInPages, bool changeCountInGenres, bool changeCountInFormats, bool changeCountInStatuses)
        {
            IQueryable<Film> films = ItemsRepository.Items
                .AsNoTracking();
            if (_currentSearch)
            {
                string property = _currentSearch.Property == "Автор"
                ? "Producer"
                : "Name";
                films = ItemsRepository.CustomFromSQLRaw(
                    string.Format("Select * FROM Films WHERE {0} COLLATE RUSSIAN_NOCASE ", property) + "LIKE {0}", $"%{_currentSearch.Value}%")
                    .Include(i => i.FilmGenres)
                    .ThenInclude(i => i.Genre);
            }    
            if (changeCountInGenres)
            {
                var genres = GetFilteredFilms(films, false, true, true);
                _filterGenre[0].Count = await genres
                    .Select(i => i.Film.Id)
                    .Distinct()
                    .CountAsync();
                var genresList = await genres
                    .GroupBy(i => i.GenreId)
                    .Select(j => new { Genr = j.Key, Count = j.Count() })
                    .ToListAsync();
                ChangeCountInFilters(_filterGenre, genresList.Select(i => i.Genr).ToList(), genresList.Select(i => i.Count).ToList());
            }
            if (changeCountInFormats)
            {
                var formats = await GetFilteredCountsAsync(films, false, true, i => i.Format);
                ChangeCountInFilters(_filterFormat, formats.Select(i => i.Key).ToList(), formats.Select(i => i.Count).ToList());
            }
            if (changeCountInStatuses)
            {
                var statuses = await GetFilteredCountsAsync(films, true, false, i => i.Status);
                ChangeCountInFilters(_filterStatus, statuses.Select(i => i.Key).ToList(), statuses.Select(i => i.Count).ToList());
            }

            films = GetFilteredFilms(films, true, true, true)
                .Select(i => i.Film)
                .Distinct();
            if (changeCountInPages)
            {
                _filmCount = await films.CountAsync();
                CountPages = GetPageCount();
                if (_inFirstPage)
                {
                    _pages.ClearAndAddElements(Enumerable.Range(1, _countPages < 10 ? _countPages : 10).ToList());
                    SelectedPage = 1;
                    _inFirstPage = false;
                }
                OnPropertyChanged(nameof(SelectedPage));
            }
            _films.ClearAndAddElements(await films
                .CustomSort(_selectedSort.Value.func, _selectedSort.Value.ascending)
                .Skip((_selectedPage - 1) * MAXCOUNT)
                .Take(MAXCOUNT).ToListAsync());
        }

        private void ChangeCountInFilters(Filter[] arrayFilter, List<int> keys, List<int> counts)
        {
            List<int> groupCounts = new(Enumerable.Range(0, arrayFilter.Length).Select(i => 0));
            for (int i = 0; i < keys.Count; i++)
                groupCounts[keys[i] + 1] = counts[i];
            int count = 0;
            for (int i = 1; i < groupCounts.Count; i++)
            {
                count += groupCounts[i];
                arrayFilter[i].Count = groupCounts[i];
            }
            arrayFilter[0].Count = count;
        }

        private void ChangeCountInFilters(ObservableCollection<Filter> collectionFilter, List<int> keys, List<int> counts)
        {
            List<int> groupCounts = new(Enumerable.Range(0, collectionFilter.Count - 1).Select(i => 0));
            for (int i = 0; i < keys.Count; i++)
                groupCounts[keys[i] - 1] = counts[i];
            for (int i = 0; i < groupCounts.Count; i++)
                collectionFilter[i + 1].Count = groupCounts[i];
        }

        #endregion

    }
}
