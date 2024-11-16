using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class MainWindowViewModel(IOpenWindows openWindows,
                                  IUserDialog userDialog,
                                  IWorkWithJSONFile workWithJSONFile,
                                  IRepository<Target> targetRepository,
                                  IRepository<TargetsGroup> targetsGroupRepository) : ViewModel
    {
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IWorkWithJSONFile _workWithJSONFile = workWithJSONFile;
        private readonly IRepository<Target> _targetRepository = targetRepository;
        private readonly IRepository<TargetsGroup> _targets = targetsGroupRepository;

        #region Propserties...

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
            set
            {
                if (!Set(ref _showHomeView, value)) return;
                ChangeCurrentView(value, () => CurrentViewModel = new HomeViewModel());
            }
        }

        #endregion

        #region ShowTargetsView : bool - Отобразить представление целей

        ///<summary>Отобразить представление целей</summary>
        private bool _showTargetsView;

        ///<summary>Отобразить представление целей</summary>
        public bool ShowTargetsView
        {
            get => _showTargetsView;
            set
            {
                if (!Set(ref _showTargetsView, value)) return;
                ChangeCurrentView(value, () => CurrentViewModel = new TargetsUCViewModel(_openWindows, _userDialog, _targetRepository, _targets));
            }
        }

        #endregion

        #region ShowTasksView : bool - Отобразить представление задач

        ///<summary>Отобразить представление задач</summary>
        private bool _showTasksView;

        ///<summary>Отобразить представление задач</summary>
        public bool ShowTasksView
        {
            get => _showTasksView;
            set
            {
                if (!Set(ref _showTasksView, value)) return;
                ChangeCurrentView(value, () => CurrentViewModel = new ListTasksUCViewModel(_workWithJSONFile, _openWindows, _userDialog));
            }
        }

        #endregion

        #region ShowChallengesView : bool - Отобразить представление челленджей

        ///<summary>Отобразить представление челленджей</summary>
        private bool _showChallengesView;

        ///<summary>Отобразить представление челленджей</summary>
        public bool ShowChallengesView
        {
            get => _showChallengesView;
            set
            {
                if (!Set(ref _showChallengesView, value)) return;
                ChangeCurrentView(value, () => CurrentViewModel = new ListChallengesUCViewModel(_openWindows, _userDialog, _workWithJSONFile));
            }
        }

        #endregion

        #region ShowBooksView : bool - Отобразить представление книг

        ///<summary>Отобразить представление книг</summary>
        private bool _showBooksView;

        ///<summary>Отобразить представление книг</summary>
        public bool ShowBooksView
        {
            get => _showBooksView;
            set
            {
                if (!Set(ref _showBooksView, value)) return;
                ChangeCurrentView(value, () => CurrentViewModel = new ListBooksUCViewModel(_userDialog, _workWithJSONFile, _openWindows));
            }
        }

        #endregion

        #region ShowCinemaView : bool - Отобразить представление кино

        ///<summary>Отобразить представление кино</summary>
        private bool _showCinemaView;

        ///<summary>Отобразить представление кино</summary>
        public bool ShowCinemaView
        {
            get => _showCinemaView;
            set
            {
                if (!Set(ref _showCinemaView, value)) return;
                ChangeCurrentView(value, () => CurrentViewModel = new CinemaUCViewModel());
            }
        }

        #endregion

        #region ShowSettingsView : bool - Отобразить представление настроек

        ///<summary>Отобразить представление настроек</summary>
        private bool _showSettingsView;

        ///<summary>Отобразить представление настроек</summary>
        public bool ShowSettingsView
        {
            get => _showSettingsView;
            set
            {
                if(!Set(ref _showSettingsView, value)) return;
                ChangeCurrentView(value, () => CurrentViewModel = new SettingsUCViewModel());
            }
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

        private void ChangeCurrentView(bool showView, Action currentViewAction)
        {
            if (showView) currentViewAction();
            else if (!(_showHomeView || _showTargetsView || _showTasksView
                    || _showChallengesView || _showBooksView || _showCinemaView
                    || _showSettingsView))
                ShowHomeView = true;
        }
    }
}
