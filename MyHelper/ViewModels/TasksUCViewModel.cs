using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Models;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal sealed class TasksUCViewModel(IRepository<MyTask> tasksRepository) : MainFunctionsViewModel<MyTask>(tasksRepository), IDisposable
    {
        private readonly ObservableCollection<MyTask> _tasks = [];
        private bool _disposed = false;
        private bool _promptFilter = false;
        private bool _importantFilter = false;
        private int _count = 0;

        #region Properties...

        private readonly CollectionViewSource _tasksViewSource = new();
        public ICollectionView TasksView => _tasksViewSource.View;

        #region Keys : ObservableCollection<string> - Список ключей

        ///<summary>Список ключей</summary>
        private ObservableCollection<string>? _keys;

        ///<summary>Список ключей</summary>
        public ObservableCollection<string>? Keys { get => _keys; set => Set(ref _keys, value); }

        #endregion

        #region NewGroup : string? - Новая группа

        ///<summary>Новая группа</summary>
        private string? _newGroup;

        ///<summary>Новая группа</summary>
        public string? NewGroup { get => _newGroup; set => Set(ref _newGroup, value); }

        #endregion

        #region SelectedTask : MyTask? - Выбранная задача

        ///<summary>Выбранная задача</summary>
        private MyTask? _selectedTask;

        ///<summary>Выбранная задача</summary>
        public MyTask? SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (!Set(ref _selectedTask, value)) return;

                OnPropertyChanged(nameof(EnableToggleButtonEditElement));
            }
        }

        #endregion

        #region Sort : Dictionary<string, SortDescription> - Сортировка

        ///<summary>Сортировка</summary>
        public Dictionary<string, SortDescription> Sort { get; } = new Dictionary<string, SortDescription>()
        {
            { "Сначала старые", new SortDescription("Id", ListSortDirection.Ascending) },
            { "Сначала свежие", new SortDescription("Id", ListSortDirection.Descending) },
            { "По задачам (Z -> Я)", new SortDescription("Name", ListSortDirection.Ascending) },
            { "По задачам (Я -> Z)", new SortDescription("Name", ListSortDirection.Descending) },
            { "По датам (ближние)", new SortDescription("End", ListSortDirection.Ascending) },
            { "По датам (дальние)", new SortDescription("End", ListSortDirection.Descending) },
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
                _tasksViewSource.SortDescriptions[0] = value.Value;
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
                Func<MyTask, bool>? func = CreateFunc(!Equals(value.Key, "Все"), !_checkedPriorityFilterAll);
                var tasks = func is not null
                    ? _itemsRepository.Items.Where(func)
                    : _itemsRepository.Items;
                foreach (MyTask task in tasks)
                    _tasks.Add(task);
            }
        }

        #endregion

        #region TaskForAdd_Edit : MyTask - задача для добавления (редактирования)

        ///<summary>задача для добавления (редактирования)</summary>
        private MyTask _taskForAdd_Edit = new() { End = DateTime.Today };

        ///<summary>задача для добавления (редактирования)</summary>
        public MyTask TaskForAdd_Edit { get => _taskForAdd_Edit; set => Set(ref _taskForAdd_Edit, value); }

        #endregion

        #region CheckedPriorityFilterAll : bool - Чек фильтра по приоритетам на "Все"

        ///<summary>Чек фильтра по приоритетам на "Все"</summary>
        private bool _checkedPriorityFilterAll = true;

        ///<summary>Чек фильтра по приоритетам на "Все"</summary>
        public bool CheckedPriorityFilterAll { get => _checkedPriorityFilterAll; set => Set(ref _checkedPriorityFilterAll, value); }

        #endregion

        #region EnableToggleButtonEditElement : bool - Включить переключатель редактирования элемента

        ///<summary>Включить переключатель редактирования элемента</summary>
        public bool EnableToggleButtonEditElement => EnableFrameworkElements && _selectedTask is not null;

        #endregion

        #endregion

        #region Commands...

        #region LoadedCommand - Команда - загрузка окна

        protected override void OnLoadedCommandExecuted(object? p)
        {
            var groupAllCount = 0;
            Dictionary<string, int> groupsCount = [];
            foreach (var task in _itemsRepository.Items)
            {
                groupAllCount++;
                if (!groupsCount.TryAdd(task.Group, 1))
                    groupsCount[task.Group]++;
            }
            Groups["Все"] = groupAllCount;
            _count = groupAllCount;
            foreach (var group in groupsCount)
                Groups.Add(group.Key, group.Value);
            SelectedGroup = Groups.GetKeyValuePair("Все");
            _tasksViewSource.SortDescriptions.Add(Sort["Сначала старые"]);
            _tasksViewSource.Source = _tasks;
            OnPropertyChanged(nameof(TasksView));
        }

        #endregion

        #region override ClosedCommand - Команда - закрыть окно

        protected override void OnClosedCommandExecuted(object? p) => Dispose();

        #endregion

        #region PriorityFilterCommand - Команда - фильтровать по приоритету

        ///<summary>Команда - фильтровать по приоритету</summary>
        private ICommand? _priorityFilterCommand;

        ///<summary>Команда - фильтровать по приоритету</summary>
        public ICommand PriorityFilterCommand => _priorityFilterCommand
            ??= new LambdaCommand<string>(OnPriorityFilterCommandExecuted, CanPriorityFilterCommandExecute);

        ///<summary>Проверка возможности выполнения - фильтровать по приоритету</summary>
        private bool CanPriorityFilterCommandExecute(string p) => _count > 10;

        ///<summary>Логика выполнения - фильтровать по приоритету</summary>
        private void OnPriorityFilterCommandExecuted(string p)
        {
            int groupAllCount = 0;
            Dictionary<string, int> groupsCount = [];

            void AddInGroup(string group)
            {
                groupAllCount++;
                if (!groupsCount.TryAdd(group, 1))
                    groupsCount[group]++;
            };

            _tasks.Clear();
            foreach (var group in Groups)
                group.Value = 0;
            if (!_checkedPriorityFilterAll)
            {
                string[] filters = p.Split('/', StringSplitOptions.RemoveEmptyEntries);
                _importantFilter = filters[0] == "Важные";
                _promptFilter = filters[1] == "Срочные";
            }
            Func<MyTask, bool>? func = CreateFunc(false, !_checkedPriorityFilterAll);
            var tasks = func is not null
                ? _itemsRepository.Items.Where(func)
                : _itemsRepository.Items;
            if (Equals(_selectedGroup.Key, "Все"))
                foreach (var task in tasks)
                {
                    AddInGroup(task.Group);
                    _tasks.Add(task);
                }
            else
                foreach (var task in tasks)
                {
                    AddInGroup(task.Group);
                    if (Equals(task.Group, _selectedGroup.Key))
                        _tasks.Add(task);
                }
            Groups["Все"] = groupAllCount;
            foreach (var group in groupsCount)
                Groups[group.Key] = group.Value;
        }

        #endregion

        #region AddElementCommand - Команда - добавить элемент

        protected override bool CanAddElementCommandExecute(object? p) => ShowAdd_EditUserControl
            && !string.IsNullOrWhiteSpace(_taskForAdd_Edit.Name)
            && !string.IsNullOrWhiteSpace(_taskForAdd_Edit.Group)
            ;

        protected override async Task OnAddElementCommandExecuted(object? p)
        {
            var addTask = new MyTask()
            {
                Name = _taskForAdd_Edit.Name,
                Important = _taskForAdd_Edit.Important,
                Prompt = _taskForAdd_Edit.Prompt,
                Group = _taskForAdd_Edit.Group,
                End = _taskForAdd_Edit.End,
                Note = _taskForAdd_Edit.Note
            };
            await _itemsRepository.AddAsync(addTask);
            if (Equals(_selectedGroup.Key, "Все")
                || Equals(_selectedGroup.Key, _taskForAdd_Edit.Group))
                _tasks.Add(addTask);
            Groups["Все"]++;
            if(!Groups.ContainsKey(_taskForAdd_Edit.Group))
                Groups.Add(_taskForAdd_Edit.Group, 0);
            Groups[_taskForAdd_Edit.Group]++;
            _count++;
            AddElement = false;
        }

        #endregion

        #region EditElementCommand - Команда - редактировать элемент

        protected override bool CanEditElementCommandExecute(object? p) => ShowAdd_EditUserControl
            && _selectedTask is not null
            && !string.IsNullOrWhiteSpace(_taskForAdd_Edit.Name)
            &&
            (!Equals(_taskForAdd_Edit.Name, _selectedTask.Name)
            || !Equals(_taskForAdd_Edit.Group, _selectedTask.Group)
            || !Equals(_taskForAdd_Edit.End, _selectedTask.End)
            || !Equals(_taskForAdd_Edit.Important, _selectedTask.Important)
            || !Equals(_taskForAdd_Edit.Prompt, _selectedTask.Prompt)
            || !Equals(_taskForAdd_Edit.Note, _selectedTask.Note))
            ;

        protected override async Task OnEditElementCommandExecuted(object? p)
        {
            if (!Equals(_taskForAdd_Edit.Group, _selectedTask.Group))
            {
                Groups[_selectedTask.Group]--;
                Groups[_taskForAdd_Edit.Group]++;
            }
            SelectedTask.Name = _taskForAdd_Edit.Name;
            SelectedTask.Important = _taskForAdd_Edit.Important;
            SelectedTask.Prompt = _taskForAdd_Edit.Prompt;
            SelectedTask.Group = _taskForAdd_Edit.Group;
            SelectedTask.Note = _taskForAdd_Edit.Note;
            await _itemsRepository.UpdateAsync(_selectedTask);
            TasksView.Refresh();
            EditElement = false;
        }

        #endregion

        #region DeleteElementCommand - Команда - удалить элемент

        protected override bool CanDeleteElementCommandExecute(object? p) => _selectedTask is not null
            && !ShowAdd_EditUserControl;

        protected override async Task OnDeleteElementCommandExecuted(object? p)
        {
            var removedTask = _selectedTask;
            if (_tasks.Remove(removedTask))
            {
                Groups["Все"]--;
                var group = _selectedGroup;
                if (Equals(group.Key, "Все"))
                    group = Groups.GetKeyValuePair(removedTask.Group);
                if (--group.Value == 0)
                    Groups.Remove(group.Key);
                await _itemsRepository.RemoveAsync(removedTask.Id);
            }
        }

        #endregion

        #region AddGroupCommand - Команда - добавить группу

        ///<summary>Команда - добавить группу</summary>
        private ICommand? _addGroupCommand;

        ///<summary>Команда - добавить группу</summary>
        public ICommand AddGroupCommand => _addGroupCommand
            ??= new LambdaCommand(OnAddGroupCommandExecuted, CanAddGroupCommandExecute);

        ///<summary>Проверка возможности выполнения - добавить группу</summary>
        private bool CanAddGroupCommandExecute(object? p) => !string.IsNullOrWhiteSpace(_newGroup);

        ///<summary>Логика выполнения - добавить группу</summary>
        private void OnAddGroupCommandExecuted(object? p)
        {
            Keys?.Add(_newGroup);
            TaskForAdd_Edit.Group = _newGroup;
            OnPropertyChanged(nameof(TaskForAdd_Edit));
            NewGroup = null;
        }

        #endregion

        #endregion

        private Func<MyTask, bool>? CreateFunc(bool filterGroup, bool filterProiority)
        {
            Func<MyTask, bool>? func = null;
            if (filterGroup && filterProiority)
                func = t => Equals(_selectedGroup.Key, t.Group)
                && t.Prompt == _promptFilter
                && t.Important == _importantFilter;
            else if (filterGroup)
                func = t => Equals(_selectedGroup.Key, t.Group);
            else if (filterProiority)
                func = t => t.Prompt == _promptFilter
                && t.Important == _importantFilter;
            return func;
        }

        protected override void MethodBeforeAddElement()
        {
            if (AddElement)
            {
                var keys = Groups.Keys;
                keys.RemoveAt(0);
                if (Keys is null || !Keys.SequenceEqual(keys))
                    Keys = new ObservableCollection<string>(keys);
                TaskForAdd_Edit.Name = string.Empty;
                TaskForAdd_Edit.Note = null;
                TaskForAdd_Edit.Prompt = false;
                TaskForAdd_Edit.Important = false;
                TaskForAdd_Edit.End = DateTime.Today;
                if (!Equals(_selectedGroup.Key, "Все")) TaskForAdd_Edit.Group = _selectedGroup.Key;
                TaskForAdd_Edit.End = DateTime.Today;
                OnPropertyChanged(nameof(TaskForAdd_Edit));
            }
            OnPropertyChanged(nameof(EnableToggleButtonEditElement));
        }

        protected override void MethodBeforeEditElement()
        {
            if (EditElement)
            {
                var keys = Groups.Keys;
                keys.RemoveAt(0);
                if (Keys is null || Keys.SequenceEqual(keys))
                    Keys = new ObservableCollection<string>(keys);
                TaskForAdd_Edit.Name = _selectedTask.Name;
                TaskForAdd_Edit.Note = _selectedTask.Note;
                TaskForAdd_Edit.Prompt = _selectedTask.Prompt;
                TaskForAdd_Edit.Important = _selectedTask.Important;
                TaskForAdd_Edit.Group = _selectedTask.Group;
                TaskForAdd_Edit.End = _selectedTask.End;
                OnPropertyChanged(nameof(TaskForAdd_Edit));
            }
            OnPropertyChanged(nameof(EnableToggleButtonEditElement));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _tasks.Clear();
                    Groups.Clear();
                    Sort.Clear();
                    Keys?.Clear();
                    Keys = null;
                }
                _disposed = true;
            }
        }
    }
}
