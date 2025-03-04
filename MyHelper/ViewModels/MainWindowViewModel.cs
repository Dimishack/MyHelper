using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class MainWindowViewModel(IUserDialog userDialog,
                                  IRepository<Target> targetRepository,
                                  IRepository<TargetsGroup> targetsGroupRepository,
                                  IRepository<MyTask> tasksRepository,
                                  IRepository<Challenge> challengeRepository,
                                  IRepository<Check> checkRepostiory,
                                  IRepository<Genre> genreRepository,
                                  IRepository<Film> filmRepository,
                                  IRepository<FilmGenre> filmGenreRepository
        ) : ViewModel
    {
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IRepository<Target> _targetRepository = targetRepository;
        private readonly IRepository<TargetsGroup> _targetsRepository = targetsGroupRepository;
        private readonly IRepository<Challenge> _challengeRepository = challengeRepository;
        private readonly IRepository<Check> _checkRepostiory = checkRepostiory;
        private readonly IRepository<Genre> _genreRepository = genreRepository;
        private readonly IRepository<Film> _filmRepository = filmRepository;
        private readonly IRepository<FilmGenre> _filmGenreRepository = filmGenreRepository;

        #region Properties...

        #region CurrentViewModel : ViewModel - Текущая модель-представления

        ///<summary>Текущая модель-представления</summary>
        private ViewModel _currentViewModel = new HomeViewModel();

        ///<summary>Текущая модель-представления</summary>
        public ViewModel CurrentViewModel { get => _currentViewModel; set => Set(ref _currentViewModel, value); }

        #endregion

        #region ShowFullMenu : bool - Показать меню полностью

        ///<summary>Показать меню полностью</summary>
        private bool _showFullMenu = false;

        ///<summary>Показать меню полностью</summary>
        public bool ShowFullMenu
        {
            get => _showFullMenu;
            set
            {
                if (!Set(ref _showFullMenu, value)) return;
                OnPropertyChanged(nameof(VisibleTooltips));
            }
        }

        #endregion

        #region VisibleTooltips : bool - видимость подсказок

        ///<summary>видимость подсказок</summary>
        public bool VisibleTooltips => !_showFullMenu;

        #endregion

        #region ShowHomeView : bool - Отобразить представление начального экрана

        ///<summary>Отобразить представление начального экрана</summary>
        private bool _showHomeView = true;

        ///<summary>Отобразить представление начального экрана</summary>
        public bool ShowHomeView
        {
            get => _showHomeView;
            set => ChangeCurrentView(ref _showHomeView, value,
                    () => CurrentViewModel = new HomeViewModel());
        }

        #endregion

        #region ShowTargetsView : bool - Отобразить представление целей

        ///<summary>Отобразить представление целей</summary>
        private bool _showTargetsView;

        ///<summary>Отобразить представление целей</summary>
        public bool ShowTargetsView
        {
            get => _showTargetsView;
            set => ChangeCurrentView(ref _showTargetsView, value,
                    () => CurrentViewModel = new TargetsUCViewModel(_targetRepository, _targetsRepository));
        }

        #endregion

        #region ShowTasksView : bool - Отобразить представление задач

        ///<summary>Отобразить представление задач</summary>
        private bool _showTasksView;

        ///<summary>Отобразить представление задач</summary>
        public bool ShowTasksView
        {
            get => _showTasksView;
            set => ChangeCurrentView(ref _showTasksView, value,
                    () => CurrentViewModel = new TasksUCViewModel(tasksRepository));
        }

        #endregion

        #region ShowChallengesView : bool - Отобразить представление челленджей

        ///<summary>Отобразить представление челленджей</summary>
        private bool _showChallengesView;

        ///<summary>Отобразить представление челленджей</summary>
        public bool ShowChallengesView
        {
            get => _showChallengesView;
            set => ChangeCurrentView(ref _showChallengesView, value,
                    () => CurrentViewModel = new ChallengesUCViewModel(_challengeRepository, _checkRepostiory));
        }

        #endregion

        #region ShowBooksView : bool - Отобразить представление книг

        ///<summary>Отобразить представление книг</summary>
        private bool _showBooksView;

        ///<summary>Отобразить представление книг</summary>
        public bool ShowBooksView
        {
            get => _showBooksView;
            set => ChangeCurrentView(ref _showBooksView, value,
                    () => CurrentViewModel = new BooksUCViewModel());
        }

        #endregion

        #region ShowFilmsView : bool - Отобразить представление фильмов

        ///<summary>Отобразить представление кино</summary>
        private bool _showFilmsView;

        ///<summary>Отобразить представление кино</summary>
        public bool ShowFilmsView
        {
            get => _showFilmsView;
            set => ChangeCurrentView(ref _showFilmsView, value,
                    () => CurrentViewModel = new FilmsUCViewModel(_filmRepository, _genreRepository, _filmGenreRepository));
        }

        #endregion

        #region ShowSettingsView : bool - Отобразить представление настроек

        ///<summary>Отобразить представление настроек</summary>
        private bool _showSettingsView;

        ///<summary>Отобразить представление настроек</summary>
        public bool ShowSettingsView
        {
            get => _showSettingsView;
            set => ChangeCurrentView(ref _showSettingsView, value,
                    () => CurrentViewModel = new SettingsUCViewModel());
        }

        #endregion

        #endregion

        #region Commands...

        #region HideMenuCommand - Команда - спрятать меню

        ///<summary>Команда - спрятать меню</summary>
        private ICommand? _hideMenuCommand;

        ///<summary>Команда - спрятать меню</summary>
        public ICommand HideMenuCommand => _hideMenuCommand
            ??= new LambdaCommand(OnHideMenuCommandExecuted, CanHideMenuCommandExecute);

        ///<summary>Проверка возможности выполнения - спрятать меню</summary>
        private bool CanHideMenuCommandExecute(object? p) => ShowFullMenu;

        ///<summary>Логика выполнения - спрятать меню</summary>
        private void OnHideMenuCommandExecuted(object? p) => ShowFullMenu = false;

        #endregion

        #endregion

        private void ChangeCurrentView(ref bool field, bool value, Action currentViewAction)
        {
            if (!Set(ref field, value) || !value) return;
            currentViewAction();
            ShowFullMenu = false;
        }
    }
}
