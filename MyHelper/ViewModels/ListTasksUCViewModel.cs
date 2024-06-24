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
    internal enum ChangeGroup
    {
        AddTask = 0,
        ModifyTask = 1,
        DeleteTask = 2
    }
    internal class ListTasksUCViewModel(
        IWorkWithJSONFile workWithJSONFile,
        IOpenWindows openWindows,
        IUserDialog userDialog) : ViewModel
    {
        private const string FILEPATH = @"Data/Tasks.json";
        private readonly IWorkWithJSONFile _workWithJSONFile = workWithJSONFile;
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;

        private bool _isLoad = true;

        #region Properties...

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

        #region Groups : Dictionary<string, int> - Группы

        ///<summary>Группы</summary>
        private Dictionary<string, int> _groups = [];

        ///<summary>Группы</summary>
        public Dictionary<string, int> Groups { get => _groups; set => Set(ref _groups, value); }

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
                if (!Set(ref _selectedGroup, value) || value is null) return;

                if (value.Contains("Все"))
                    _listTasksView.Source = new ObservableCollection<MyTask>(_listTasks);
                else
                    _listTasksView.Source = new ObservableCollection<MyTask>(_listTasks.Where(
                        i =>
                        !string.IsNullOrWhiteSpace(i.Group) &&
                        value.Contains(i.Group)));
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

        #region Commands...

        #region LoadedCommand - Команда - загрузка окна

        ///<summary>Команда - загрузка окна</summary>
        private ICommand? _loadedCommand;

        ///<summary>Команда - загрузка окна</summary>
        public ICommand LoadedCommand => _loadedCommand
            ??= new LambdaCommandAsync(OnLoadedCommandExecuted);

        ///<summary>Логика выполнения - загрузка окна</summary>
        private async Task OnLoadedCommandExecuted(object? p)
        {
            if (!_isLoad) return;

            ObservableCollection<MyTask>? tasks;
            if ((tasks = await _workWithJSONFile.ReadFileAsync<ObservableCollection<MyTask>>(FILEPATH)) is not null)
            {
                ListTasks = new(tasks);
                ((Command)SaveTasksCommand).Executable = false;
            }
            SelectedSorting = "Сначала старые записи";
            Groups.Add("Все", ListTasks.Count);
            foreach (var group in _listTasks.GroupBy(i => i.Group).OrderBy(j => j.Key))
            {
                if (string.IsNullOrWhiteSpace(group.Key)) continue;
                Groups.Add(group.Key, group.Count());
            }
            CollectionViewSource.GetDefaultView(Groups).Refresh();
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
            if (!_openWindows.OpenCreator_EditorTaskWindow(newTask, _groups.GetKeys(), "Добавить задачу")) return;

            ListTasks.Add(newTask);
            ((ObservableCollection<MyTask>)_listTasksView.Source).Add(newTask);
            OnChangingGroups(ChangeGroup.AddTask, newTask.Group);
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
            var oldGroup = p!.Group;
            if (!_openWindows.OpenCreator_EditorTaskWindow(p, _groups.GetKeys(), "Редактировать задачу")) return;

            if (oldGroup != p.Group)
                OnChangingGroups(ChangeGroup.ModifyTask, p.Group, oldGroup);

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
            var index = _listTasks.IndexOf(p!);
            OnChangingGroups(ChangeGroup.DeleteTask, p?.Group);
            ListTasks.Remove(p!);
            ((ObservableCollection<MyTask>)_listTasksView.Source).Remove(p);
            for (int i = index; i < _listTasks.Count; i++)
                ListTasks[i].Id--;
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
            //for (int i = 0; i < _listTasks.Count; i++)
            //{
            //    ListTasks[i].Id = i;
            //}
            if (!await _workWithJSONFile.WriteFileAsync(FILEPATH, _listTasks)) return;

            _userDialog.InformationMessage("Список задач успешно сохранен");
            ((Command)SaveTasksCommand).Executable = false;
        }

        #endregion

        #endregion

        #region Methods...

        private void OnChangingGroups(ChangeGroup changeGroup, string? group = null, string? oldGroup = null)
        {
            switch (changeGroup)
            {
                case ChangeGroup.AddTask:
                    if (group is not null)
                    {
                        if (!_groups.ContainsKey(group))
                            Groups.Add(group, 0);
                        Groups[group]++;
                    }
                    Groups["Все"]++;
                    break;
                case ChangeGroup.ModifyTask:
                    if (group is not null)
                    {
                        if (oldGroup is not null)
                        {
                            Groups[oldGroup]--;
                            if (Groups[oldGroup] <= 0)
                                Groups.Remove(oldGroup);
                        }
                        if (!_groups.ContainsKey(group))
                            Groups.Add(group, 0);
                        Groups[group]++;
                    }
                    break;
                case ChangeGroup.DeleteTask:
                    if (group is not null)
                    {
                        Groups[group]--;
                        if (Groups[group] <= 0)
                            Groups.Remove(group);
                    }
                    Groups["Все"]--;
                    break;
                default:
                    break;
            }
            CollectionViewSource.GetDefaultView(Groups).Refresh();
        }

        #endregion
    }
}
