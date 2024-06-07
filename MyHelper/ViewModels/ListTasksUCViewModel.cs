using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Models.MyTasks;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ListTasksUCViewModel(IWorkWithJSONFile workWithJSONFile,
        IOpenWindows openWindows,
        IUserDialog userDialog) : ViewModel
    {
        private const string FILEPATH = @"Data/Tasks.json"; 
        private readonly IWorkWithJSONFile _workWithJSONFile = workWithJSONFile;
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;

        private bool _isLoad = true;

        #region Свойства

        public Dictionary<string, SortDescription> Sorting { get; } = new()
        {
            {"Сначала старые записи", new("Id", ListSortDirection.Ascending) },
            {"Сначала новые записи", new("Id", ListSortDirection.Descending) },
            {"Задачи (по возрастанию)", new("Task", ListSortDirection.Ascending) },
            {"Задачи (по убыванию)", new("Task", ListSortDirection.Descending) },
            {"Сначала важные", new("Prompt", ListSortDirection.Descending) },
            {"Сначала срочные", new("Important", ListSortDirection.Descending) },
            {"Срок (по возрастанию)", new("Term", ListSortDirection.Ascending) },
            {"Срок (по убыванию)", new("Term", ListSortDirection.Descending) },
        };

        #region SelectedSorting : string - Выбранная сортировка

        ///<summary>Выбранная сортировка</summary>
        private string _selectedSorting = string.Empty;

        ///<summary>Выбранная сортировка</summary>
        public string SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                if (!Set(ref _selectedSorting, value)) return;

                _listTasksView.SortDescriptions.Clear();
                _listTasksView.SortDescriptions.Add(Sorting[value]);
            }
        }

        #endregion

        #region Groups : IList<string> - Группы

        ///<summary>Группы</summary>
        private IList<string> _groups = ["Все"];

        ///<summary>Группы</summary>
        public IList<string> Groups { get => _groups; set => Set(ref _groups, value); }

        #endregion

        #region SelectedGroup : string - Выбранная группа

        ///<summary>Выбранная группа</summary>
        private string _selectedGroup = string.Empty;

        ///<summary>Выбранная группа</summary>
        public string SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (!Set(ref _selectedGroup, value)) return;

                if (value.Contains("Все"))
                    _listTasksView.Source = new ObservableCollection<MyTask>(_listTasks);
                else
                    _listTasksView.Source = new ObservableCollection<MyTask>(_listTasks?.Where(i => i.Group == value));
                OnPropertyChanged(nameof(ListTasksView));
            }
        }

        #endregion

        #region ListTasks : ObservableCollection<MyTasks> - Список задач

        ///<summary>Список задач</summary>
        private ObservableCollection<MyTask> _listTasks = [];

        ///<summary>Список задач</summary>
        public ObservableCollection<MyTask> ListTasks { get => _listTasks; set => Set(ref _listTasks, value); }

        #endregion

        private readonly CollectionViewSource _listTasksView = new();
        public ICollectionView ListTasksView => _listTasksView.View;

        #region SelectedTask : MyTask? - Выбранная задача

        ///<summary>Выбранная задача</summary>
        private MyTask? _selectedTask;

        ///<summary>Выбранная задача</summary>
        public MyTask? SelectedTask { get => _selectedTask; set => Set(ref _selectedTask, value); }

        #endregion

        #endregion

        #region Команды

        #region LoadedCommand - Команда - загрузка окна

        ///<summary>Команда - загрузка окна</summary>
        private ICommand? _loadedCommand;

        ///<summary>Команда - загрузка окна</summary>
        public ICommand LoadedCommand => _loadedCommand
            ??= new LambdaCommand(OnLoadedCommandExecuted);

        ///<summary>Логика выполнения - загрузка окна</summary>
        private void OnLoadedCommandExecuted(object? p)
        {
            if (!_isLoad) return;

            var groups = Enumerable.Range(0, 10).Select(i => $"Group {i}").ToList();
            if (_workWithJSONFile.ReadFile(FILEPATH, out IList<MyTask>? listTasks) && listTasks is not null)
            {
                ListTasks = new(listTasks);
                ((Command)SaveTasksCommand).Executable = false;
            }
            else
            {
                ListTasks = new(Enumerable.Range(0, 10000).Select(i => new MyTask
                {
                    Id = i,
                    Task = $"Task {i}",
                    Group = groups[Random.Shared.Next(0, groups.Count)],
                }).ToList());
            }
            SelectedSorting = "Сначала старые записи";
            foreach (var group in _listTasks.Select(i => i.Group).Distinct().Order().ToList())
            {
                Groups.Add(group);
            }
            SelectedGroup = "Все";
            _isLoad = false;
        }

        #endregion

        #region AddNewTaskCommand - Команда - добавить новую задачу

        ///<summary>Команда - добавить новую задачу</summary>
        private ICommand? _addNewTaskCommand;

        ///<summary>Команда - добавить новую задачу</summary>
        public ICommand AddNewTaskCommand => _addNewTaskCommand
            ??= new LambdaCommand(OnAddNewTaskCommandExecuted);

        ///<summary>Логика выполнения - добавить новую задачу</summary>
        private void OnAddNewTaskCommandExecuted(object? p)
        {
            var newTask = new MyTask() { Id = _listTasks.Count };
            if (!_openWindows.OpenCreator_EditorTaskWindow(newTask, _groups, "Добавить задачу")) return;

            ListTasks.Add(newTask);
            ((ObservableCollection<MyTask>)_listTasksView.Source).Add(newTask);
            ListTasksView.Refresh();
            _userDialog.InformationMessage("Задача успешно добавлена!");
            ((Command)SaveTasksCommand).Executable = true;

        }

        #endregion

        #region EditTaskCommand - Команда - редактировать задачу

        ///<summary>Команда - редактировать задачу</summary>
        private ICommand? _editTaskCommand;

        ///<summary>Команда - редактировать задачу</summary>
        public ICommand EditTaskCommand => _editTaskCommand
            ??= new LambdaCommand<MyTask?>(OnEditTaskCommandExecuted, CanEditTaskCommandExecute);

        ///<summary>Проверка возможности выполнения - редактировать задачу</summary>
        private bool CanEditTaskCommandExecute(MyTask? p) => p is not null;

        ///<summary>Логика выполнения - редактировать задачу</summary>
        private void OnEditTaskCommandExecuted(MyTask? p)
        {
            if (!_openWindows.OpenCreator_EditorTaskWindow(p!, _groups, "Редактировать задачу")) return;

            _listTasksView.View.Refresh();
            _userDialog.InformationMessage("Задача упешно отредактирована!");
            ((Command)SaveTasksCommand).Executable = true;
        }

        #endregion

        #region DeleteTaskCommand - Команда - удалить задачу

        ///<summary>Команда - удалить задачу</summary>
        private ICommand? _deleteTaskCommand;

        ///<summary>Команда - удалить задачу</summary>
        public ICommand DeleteTaskCommand => _deleteTaskCommand
            ??= new LambdaCommand<MyTask?>(OnDeleteTaskCommandExecuted, CanDeleteTaskCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить задачу</summary>
        private bool CanDeleteTaskCommandExecute(MyTask? p) => p is not null;

        ///<summary>Логика выполнения - удалить задачу</summary>
        private void OnDeleteTaskCommandExecuted(MyTask? p)
        {
            ListTasks.Remove(p);
            ((ObservableCollection<MyTask>)_listTasksView.Source).Remove(p);
            ((Command)SaveTasksCommand).Executable = true;
        }

        #endregion

        #region SaveTasksCommand - Команда - сохранить список задач

        ///<summary>Команда - сохранить список задач</summary>
        private ICommand? _saveTasksCommand;

        ///<summary>Команда - сохранить список задач</summary>
        public ICommand SaveTasksCommand => _saveTasksCommand
            ??= new LambdaCommandAsync(OnSaveTasksCommandExecuted);

        ///<summary>Логика выполнения - сохранить список задач</summary>
        private async Task OnSaveTasksCommandExecuted(object? p)
        {
            if(!await _workWithJSONFile.WriteFileAsync(FILEPATH, _listTasks)) return;

            _userDialog.InformationMessage("Список задач успешно сохранен");
            ((Command)SaveTasksCommand).Executable = false;
        }

        #endregion

        #endregion

    }
}
