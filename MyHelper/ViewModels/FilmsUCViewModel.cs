using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Extensions;
using MyHelper.Interfaces;
using MyHelper.Models;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal sealed class FilmsUCViewModel(IRepository<Film> filmRepository,
                                           IRepository<Genre> genreRepository,
                                           IRepository<FilmGenre> filmGenreRepository) : MainFunctionsViewModel<Film>(filmRepository)
    {
        const int MAXCOUNT = 50;

        private event EventHandler? SelectedFilmsChanged;

        private readonly IRepository<Genre> _genreRepository = genreRepository;
        private readonly IRepository<FilmGenre> _filmGenreRepository = filmGenreRepository;
        private readonly ObservableCollection<Film> _films = [];
        private readonly ObservableCollection<int> _pages = [];
        private readonly Filter[] _filterFormat = [new ("Все", true), new("Полнометражный фильм"), new("Короткометражный фильм"), new("Сериал"), new("Мини-сериал"),
                                                   new("Телевизионный фильм"), new("Веб-сериал"), new("Реалити-шоу"), new("Ток-шоу"), new("Концерт"), new("Музыкальное видео")];
        private readonly Filter[] _filterStatus = [new("Все", true), new("Запланирован"), new("Смотрю"), new("Посмотрен")];
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
        private readonly string[] _searches = ["Автор", "Название фильма"];

        private Expression<Func<Film, bool>> _funcForSearch = f => true;
        private int _filmCount = 0;
        private int _selectedFilterFormat = -1;
        private int _selectedFilterStatus = -1;
        private bool _changePages = false;
        private bool _changeCountInFormats = false;
        private bool _changeCountInStatuses = false;

        #region Properties...

        public IReadOnlyCollection<Film> FilmsCollectionView => _films;
        public IReadOnlyCollection<int> PagesCollectionView => _pages;
        public IReadOnlyDictionary<string, (Expression<Func<Film, object>> func, bool ascending)> Sorts => _sorts;
        public IReadOnlyList<Filter> FilterFormat => _filterFormat;
        public IReadOnlyList<Filter> FilterStatus => _filterStatus;
        public IReadOnlyList<string> Searches => _searches;

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
                    _pages.ClearAndAddElements(Enumerable.Range(1, 10).ToList());
                    OnPropertyChanged(nameof(SelectedPage));
                }
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

                if (_selectedPage > 1) SelectedPage = 1;
                else OnSelectedFilmsChanged();
            }
        }

        #endregion

        #region FieldSearch : string - поле поиска

        ///<summary>поле поиска</summary>
        private string _fieldSearch = string.Empty;

        ///<summary>поле поиска</summary>
        public string FieldSearch { get => _fieldSearch; set => Set(ref _fieldSearch, value); }

        #endregion

        #region SelectedSearch : string? - выбранный поиск

        ///<summary>выбранный поиск</summary>
        private string? _selectedSearch;

        ///<summary>выбранный поиск</summary>
        public string? SelectedSearch { get => _selectedSearch; set => Set(ref _selectedSearch, value); }

        #endregion

        #endregion

        #region Commands...

        protected override void OnLoadedCommandExecuted(object? p)
        {
        }

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
            SelectedFilmsChanged += FilmsUCViewModel_SelectedFilmsChanged;
            var films = _itemsRepository.Items.AsNoTracking();
            var test = await films.GroupBy(f => f.Format).Select(g => new { Format = g.Key, Count = g.Count() }).ToListAsync();
            var test2 = await films.GroupBy(f => f.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync();
            var changeCountTasks = new List<Task>()
            {
                Task.Run(() => ChangeCountInFilters(_filterFormat, test.Select(i => i.Format).ToList(), test.Select(i => i.Count).ToList())),
                Task.Run(() => ChangeCountInFilters(_filterStatus, test2.Select(i => i.Status).ToList(), test2.Select(i => i.Count).ToList()))
            };
            await Task.WhenAll(changeCountTasks);
            _filmCount = _filterStatus[0].Count;
            _films.ClearAndAddElements(await films.Take(MAXCOUNT).ToListAsync());
            int countPage = GetPageCount();
            _pages.ClearAndAddElements(Enumerable.Range(1, countPage < 10 ? countPage : 10).ToList());
        }

        #endregion

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
            ??= new LambdaCommandAsync<string>(OnFilterByFormatCommandExecuted);

        ///<summary>Логика выполнения - фильтровать по формату</summary>
        private async Task OnFilterByFormatCommandExecuted(string p)
        {
            _selectedFilterFormat = Array.FindIndex(_filterFormat, f => f.Name == p) - 1;
            _changePages = true;
            _changeCountInStatuses = true;
            if (_selectedPage > 1) SelectedPage = 1;
            else await SetFilmsAsync();
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
            _changePages = true;
            _changeCountInFormats = true;
            if (_selectedPage > 1) SelectedPage = 1;
            else await SetFilmsAsync();
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
            !string.IsNullOrWhiteSpace(_fieldSearch) && !string.IsNullOrEmpty(_selectedSearch);

        ///<summary>Логика выполнения - поиск</summary>
        private async Task OnSearchCommandExecuted(object? p)
        {
            _funcForSearch = GetFuncBySearch(_selectedSearch);
            _changePages = true;
            _changeCountInFormats = true;
            _changeCountInStatuses = true;
            if (_selectedPage > 1) SelectedPage = 1;
            else await SetFilmsAsync();
        }

        #endregion

        #region CancelSearchCommand - Команда - отменить поиск

        ///<summary>Команда - отменить поиск</summary>
        private ICommand? _cancelSearchCommand;

        ///<summary>Команда - отменить поиск</summary>
        public ICommand CancelSearchCommand => _cancelSearchCommand
            ??= new LambdaCommandAsync(OnCancelSearchCommandExecuted, CanCancelSearchCommandExecute);

        ///<summary>Проверка возможности выполнения - отменить поиск</summary>
        private bool CanCancelSearchCommandExecute(object? p) => _funcForSearch is not null;

        ///<summary>Логика выполнения - отменить поиск</summary>
        private async Task OnCancelSearchCommandExecuted(object? p)
        {
            _funcForSearch = f => true;
            SelectedSearch = null;
            FieldSearch = string.Empty;
            _changePages = true;
            _changeCountInFormats = true;
            _changeCountInStatuses = true;
            if (_selectedPage > 1) SelectedPage = 1;
            else await SetFilmsAsync();
        }

        #endregion

        #endregion

        #region Events...

        private async void FilmsUCViewModel_SelectedFilmsChanged(object? sender, EventArgs e) => await SetFilmsAsync();


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
            => _filmCount / MAXCOUNT + (_filmCount % MAXCOUNT == 0 ? 0 : 1);

        private async Task SetFilmsAsync()
        {
            // Выносим проверки за пределы запроса
            bool applyFormatFilter = _selectedFilterFormat != -1;
            bool applyStatusFilter = _selectedFilterStatus != -1;

            IQueryable<Film> films = _itemsRepository.Items.AsNoTracking().Where(_funcForSearch);
            var changeCountTasks = new List<Task>();
            if (_changeCountInFormats)
            {
                var test = await films
                    .Where(i => !applyStatusFilter || i.Status == _selectedFilterStatus)
                    .GroupBy(f => f.Format)
                    .Select(g => new { Format = g.Key, Count = g.Count() })
                    .ToListAsync();
                changeCountTasks.Add(Task.Run(() => ChangeCountInFilters(_filterFormat, test.Select(i => i.Format).ToList(), test.Select(i => i.Count).ToList())));
                _changeCountInFormats = false;
            }
            if (_changeCountInStatuses)
            {
                var test2 = await films
                    .Where(i => !applyFormatFilter || i.Format == _selectedFilterFormat)
                    .GroupBy(f => f.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();
                changeCountTasks.Add(Task.Run(() => ChangeCountInFilters(_filterStatus, test2.Select(i => i.Status).ToList(), test2.Select(i => i.Count).ToList())));
                _changeCountInStatuses = false;
            }

            await Task.WhenAll(changeCountTasks);

            films = films.Where(f =>
                (!applyFormatFilter || f.Format == _selectedFilterFormat)
                && (!applyStatusFilter || f.Status == _selectedFilterStatus));
            if (_changePages)
            {
                _filmCount = await films.CountAsync();
                int countPage = GetPageCount();
                _pages.ClearAndAddElements(Enumerable.Range(1, countPage < 10 ? countPage : 10).ToList());
                OnPropertyChanged(nameof(SelectedPage));
                _changePages = false;
            }
            _films.ClearAndAddElements(await films
                .Sort(_selectedSort.Value.func, _selectedSort.Value.ascending)
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

        private Expression<Func<Film, bool>> GetFuncBySearch(string selectedSearch)
            => selectedSearch.Contains("Автор")
            ? f => f.Producer.Contains(_fieldSearch)
            : selectedSearch.Contains("Название фильма")
            ? f => f.Name.Contains(_fieldSearch)
            : f => true;


        #endregion

    }
}
