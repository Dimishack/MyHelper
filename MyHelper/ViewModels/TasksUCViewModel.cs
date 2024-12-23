using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Models;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class TasksUCViewModel(IRepository<MyTask> tasksRepository) : ViewModel
    {
        private readonly IRepository<MyTask> _tasksRepository = tasksRepository;
        private readonly ObservableCollection<MyTask> _tasks = [];


        #region Properties...

        private readonly CollectionViewSource _tasksViewSource = new();
        public ICollectionView TasksView => _tasksViewSource.View;

        #region SelectedTask : MyTask? - Выбранная задача

        ///<summary>Выбранная задача</summary>
        private MyTask? _selectedTask;

        ///<summary>Выбранная задача</summary>
        public MyTask? SelectedTask { get => _selectedTask; set => Set(ref _selectedTask, value); }

        #endregion

        #region Sort : Dictionary<string, SortDescription> - Сортировка

        ///<summary>Сортировка</summary>
        public IReadOnlyDictionary<string, SortDescription> Sort { get; } = new Dictionary<string, SortDescription>()
        {
            { "Сначала старые", new SortDescription("Id", ListSortDirection.Ascending) },
            { "Сначала свежие", new SortDescription("Id", ListSortDirection.Descending) },
            { "По задачам (Z -> Я)", new SortDescription("Name", ListSortDirection.Ascending) },
            { "По задачам (Я -> Z)", new SortDescription("Name", ListSortDirection.Descending) },
            { "По датам (ближние)", new SortDescription("DateEnd", ListSortDirection.Ascending) },
            { "По датам (дальние)", new SortDescription("DateEnd", ListSortDirection.Descending) },
        };

        #endregion

        #region SelectedSort : KeyValuePair<string, SortDescription> - Выбранная сортировка

        ///<summary>Выбранная сортировка</summary>
        private KeyValuePair<string, SortDescription> _selectedSort = new("Сначала старые", new SortDescription("Id", ListSortDirection.Ascending));

        ///<summary>Выбранная сортировка</summary>
        public KeyValuePair<string, SortDescription> SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (!Set(ref _selectedSort, value)) return;
                _tasksViewSource.SortDescriptions.Insert(0, value.Value);
            }
        }

        #endregion

        #region Groups : ObservableDictionary<string,int> - Группы

        ///<summary>Группы</summary>
        public ObservableDictionary<string, int> Groups { get; } = new() { { "Все", 0 } };

        #endregion

        #region SelectedGroup : ObservableKeyValuePair<string, int>? - Выбранная группа

        ///<summary>Выбранная группа</summary>
        private ObservableKeyValuePair<string, int>? _selectedGroup;

        ///<summary>Выбранная группа</summary>
        public ObservableKeyValuePair<string, int>? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (!Set(ref _selectedGroup, value) || value is null) return;
                _tasks.Clear();
                switch (value.Key)
                {
                    case "Все":
                        foreach (var myTask in _tasksRepository.Items)
                            _tasks.Add(myTask);
                        break;
                    default:
                        foreach (var myTask in _tasksRepository.Items.Where(t => Equals(value.Key, t.Group)))
                            _tasks.Add(myTask);
                        break;
                }
            }
        }

        #endregion

        #endregion

        #region Commands...

        #region LoadCommand - Команда - загрузка пользовательского окна

        ///<summary>Команда - загрузка пользовательского окна</summary>
        private ICommand? _loadCommand;

        ///<summary>Команда - загрузка пользовательского окна</summary>
        public ICommand LoadCommand => _loadCommand
            ??= new LambdaCommand(OnLoadCommandExecuted, CanLoadCommandExecute);

        ///<summary>Проверка возможности выполнения - загрузка пользовательского окна</summary>
        private bool CanLoadCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - загрузка пользовательского окна</summary>
        private void OnLoadCommandExecuted(object? p)
        {
            foreach (var task in _tasksRepository.Items)
            {
                _tasks.Add(task);
                Groups["Все"]++;
                if (Groups.ContainsKey(task.Group))
                    Groups[task.Group]++;
                else Groups.Add(task.Group, 1);
            }
            SelectedGroup = Groups.GetKeyValuePair("Все");
            _tasksViewSource.Source = _tasks;
            OnPropertyChanged(nameof(TasksView));
        }

        #endregion

        #region RemoveTaskCommand - Команда - удалить задачу

        ///<summary>Команда - удалить задачу</summary>
        private ICommand? _removeTaskCommand;

        ///<summary>Команда - удалить задачу</summary>
        public ICommand RemoveTaskCommand => _removeTaskCommand
            ??= new LambdaCommand(OnRemoveTaskCommandExecuted, CanRemoveTaskCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить задачу</summary>
        private bool CanRemoveTaskCommandExecute(object? p) => _selectedTask is not null;

        ///<summary>Логика выполнения - удалить задачу</summary>
        private void OnRemoveTaskCommandExecuted(object? p)
        {
            var removedTask = _selectedTask;
            if (_tasks.Remove(removedTask))
            {
                Groups["Все"]--;
                var group = _selectedGroup;
                if (Equals(group.Key, "Все"))
                    group = Groups.GetKeyValuePair(removedTask.Group);
                if(--group.Value == 0)
                    Groups.Remove(group.Key);
            }
        }

        #endregion

        #endregion
    }
}
