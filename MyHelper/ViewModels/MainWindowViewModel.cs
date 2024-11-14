using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Models.Books;
using MyHelper.Models.Challenges;
using MyHelper.Models.MyTasks;
using MyHelper.Models.Purposes;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        private readonly IOpenWindows _openWindows;
        private readonly IUserDialog _userDialog;
        private readonly IWorkWithJSONFile _workWithJSONFile;
        private readonly IRepository<TargetsGroup> _targets;

        #region CurrentViewModel : ViewModel - Текущая модель-представления

        ///<summary>Текущая модель-представления</summary>
        private ViewModel _currentViewModel;

        ///<summary>Текущая модель-представления</summary>
        public ViewModel CurrentViewModel { get => _currentViewModel; set => Set(ref _currentViewModel, value); }

        #endregion

        #region Commands...

        #region ShowTargetsViewCommand - Команда - Отобразить представление целей

        ///<summary>Команда - Отобразить представление целей</summary>
        private ICommand? _ShowTargetsViewCommand;

        ///<summary>Команда - Отобразить представление целей</summary>
        public ICommand ShowTargetsViewCommand => _ShowTargetsViewCommand
            ??= new LambdaCommand(OnShowTargetsViewCommandExecuted);
        ///<summary>Логика выполнения - Отобразить представление целей</summary>
        private void OnShowTargetsViewCommandExecuted(object? p) 
            => CurrentViewModel = new TartetsUCViewModel(_openWindows, _userDialog, _workWithJSONFile, _targets);

        #endregion

        #region ShowTasksViewCommand - Команда - Отобразить представление задач

        ///<summary>Команда - Отобразить представление задач</summary>
        private ICommand? _showTasksViewCommand;

        ///<summary>Команда - Отобразить представление задач</summary>
        public ICommand ShowTasksViewCommand => _showTasksViewCommand
            ??= new LambdaCommand(OnShowTasksViewCommandExecuted);

        ///<summary>Логика выполнения - Отобразить представление задач</summary>
        private void OnShowTasksViewCommandExecuted(object? p) 
            => CurrentViewModel = new ListTasksUCViewModel(_workWithJSONFile, _openWindows, _userDialog);

        #endregion

        #endregion

        public MainWindowViewModel(IOpenWindows openWindows,
                                      IUserDialog userDialog,
                                      IWorkWithJSONFile workWithJSONFile,
                                      IRepository<TargetsGroup> targets)
        {
            _openWindows = openWindows;
            _userDialog = userDialog;
            _workWithJSONFile = workWithJSONFile;
            _targets = targets;
            _currentViewModel = new TartetsUCViewModel(_openWindows, _userDialog, _workWithJSONFile, _targets);
        }
    }
}
