using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Attributes;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Models.Targets;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class TargetsUCViewModel(IRepository<Target> targetRepository,
                             IRepository<TargetsGroup> targetsGroupRepository) : ViewModel, IDisposable
    {
        private readonly IRepository<Target> _targetRepository = targetRepository;
        private readonly IRepository<TargetsGroup> _targetsGroupRepository = targetsGroupRepository;
        private bool _disposed = false;
        private string _filter = "все";
        private int _completedTargetsCount_Calculated = 0;

        #region Properties...

        #region GroupsTargets : ObservableCollection<TargetsModel> - Список групп с целями

        ///<summary>Список групп с целями</summary>
        public ObservableCollection<TargetsModel> GroupsTargets { get; } = [];

        #endregion

        #region SelectedTargetsGroup : TargetsModel - Выбранная группа

        ///<summary>Выбранная группа</summary>
        private TargetsModel? _selectedTargetsGroup;

        ///<summary>Выбранная группа</summary>
        public TargetsModel? SelectedTargetsGroup
        {
            get => _selectedTargetsGroup;
            set
            {
                if (_selectedTargetsGroup == value) return;
                ClearTargets();
                if (value is not null)
                {
                    if (value.Targets.Count == 0)
                    {
                        var targets = _targetsGroupRepository.Items.Include(g => g.Targets).FirstOrDefault(ts => ts.Id == value.Id)?.Targets;
                        if (targets is not null)
                        {
                            foreach (var target in targets)
                                value.Targets.Add(new TargetModel(target));
                        }
                    }
                }
                Set(ref _selectedTargetsGroup, value);
                if (value is not null)
                    foreach (var target in value.Targets)
                        Targets.Add(target);
                CompletedTargetsCount = _completedTargetsCount_Calculated;
                PropertiesChanged(this);

            }
        }

        #endregion

        #region Targets : ObservableCollection<TargetModel> - Цели

        ///<summary>Цели</summary>
        public ObservableCollection<TargetModel> Targets { get; } = [];

        #endregion

        #region SelectedTarget : TargetModel - Выбранная цель

        ///<summary>Выбранная цель</summary>
        private TargetModel? _selectedTarget;

        ///<summary>Выбранная цель</summary>
        public TargetModel? SelectedTarget
        {
            get => _selectedTarget;
            set
            {
                if (!Set(ref _selectedTarget, value)) return;
                PropertiesChanged(this);
            }
        }

        #endregion

        private readonly CollectionViewSource _selectedTargetsViewSource = new();

        [DependencyOn(nameof(SelectedTargetsGroup))]
        public ICollectionView SelectedTargetsView => _selectedTargetsViewSource.View;

        #region Sorts : Dictionary<string, SortDescription> - Список сортировки

        /// <summary> Список сортировки </summary>
        public Dictionary<string, SortDescription> Sorts { get; } = new()
        {
            {"В порядке возрастания", new SortDescription("Id", ListSortDirection.Ascending)},
            {"В порядке убывания", new SortDescription("Id", ListSortDirection.Descending)},
            {"По целям (Z->Я)", new SortDescription("Name", ListSortDirection.Ascending)},
            {"По целям (Я->Z)", new SortDescription("Name", ListSortDirection.Descending)},
        };

        #endregion

        #region SelectedSort : object - Выбранная сортировки

        ///<summary>Выбранная сортировки</summary>
        private KeyValuePair<string, SortDescription> _selectedSort;

        ///<summary>Выбранная сортировки</summary>
        public KeyValuePair<string, SortDescription> SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (!Set(ref _selectedSort, value)) return;

                if (_selectedTargetsViewSource.SortDescriptions.Count > 0)
                    _selectedTargetsViewSource.SortDescriptions.Insert(0, Sorts[value.Key]);
                _selectedTargetsViewSource.View?.Refresh();
            }
        }

        #endregion

        #region AddGroup : bool - Добавить группу

        ///<summary>Добавить группу</summary>
        private bool _addGroup;
        ///<summary>Добавить группу</summary>
        public bool AddGroup
        {
            get => _addGroup;
            set
            {
                if (!Set(ref _addGroup, value)) return;
                if (value)
                {
                    TargetsGroupForAdd_Edit.Year = GroupsTargets.Last().Year == 0 ? (uint)DateTime.Now.Year : GroupsTargets.Last().Year + 1;
                    TargetsGroupForAdd_Edit.Name = string.Empty;
                    OnPropertyChanged(nameof(TargetsGroupForAdd_Edit));
                }
                PropertiesChanged(this);
            }
        }

        #endregion

        #region AddTarget : bool - Добавить цель

        ///<summary>Добавить цель</summary>
        private bool _addTarget;
        ///<summary>Добавить цель</summary>
        public bool AddTarget
        {
            get => _addTarget;
            set
            {
                if (!Set(ref _addTarget, value)) return;
                if (value)
                {
                    TargetForAdd_Edit.Name = "";
                    TargetForAdd_Edit.Note = "";
                    TargetForAdd_Edit.IsComplete = false;
                    OnPropertyChanged(nameof(TargetForAdd_Edit));
                }
                PropertiesChanged(this);
            }
        }

        #endregion

        #region ChangeGroup : bool - Изменить группу

        ///<summary>Изменить группу</summary>
        private bool _changeGroup;
        ///<summary>Изменить группу</summary>
        public bool ChangeGroup
        {
            get => _changeGroup;
            set
            {
                if (!Set(ref _changeGroup, value)) return;
                if (value && _selectedTargetsGroup is not null)
                {
                    TargetsGroupForAdd_Edit.Year = _selectedTargetsGroup.Year;
                    TargetsGroupForAdd_Edit.Name = _selectedTargetsGroup.Name;
                    OnPropertyChanged(nameof(TargetsGroupForAdd_Edit));
                }
                PropertiesChanged(this);
            }
        }

        #endregion

        #region ChangeTarget : bool - Изменить цель

        ///<summary>Изменить цель</summary>
        private bool _changeTarget;
        ///<summary>Изменить цель</summary>
        public bool ChangeTarget
        {
            get => _changeTarget;
            set
            {
                if (!Set(ref _changeTarget, value)) return;
                if (value && _selectedTarget is not null)
                {
                    TargetForAdd_Edit.Name = _selectedTarget.Name;
                    TargetForAdd_Edit.Note = _selectedTarget.Note;
                    OnPropertyChanged(nameof(TargetForAdd_Edit));
                }
                PropertiesChanged(this);
            }
        }

        #endregion

        #region IsVisibleAdd_EditGroup : bool - Видимость окна создания или редактирования группы

        [DependencyOn([nameof(AddGroup), nameof(ChangeGroup)])]
        [ChangesWithProperties(nameof(EnableElemetns), true)]
        public bool IsVisibleAdd_EditGroup => _addGroup || _changeGroup;

        #endregion

        #region IsVisibleAdd_EditTarget : bool - Видимость окна создания или редактирования цели

        [DependencyOn([nameof(AddTarget), nameof(ChangeTarget)])]
        [ChangesWithProperties(nameof(EnableElemetns), true)]
        ///<summary>Видимость окна создания или редактирования цели</summary>
        public bool IsVisibleAdd_EditTarget => _addTarget || _changeTarget;

        #endregion

        #region EnableElemetns : bool - Включить переключатели

        [ChangesWithProperties([nameof(EnableToggleButtonCreateTarget),
            nameof(EnableToggleButtonChangeTarget), nameof(EnableToggleButtonChangeGroup)])]
        ///<summary>Включить переключатели</summary>
        public bool EnableElemetns => !IsVisibleAdd_EditGroup && !IsVisibleAdd_EditTarget;

        #endregion

        #region EnableToggleButtonChangeGroup : bool - Включить переключатель изменения групп

        [DependencyOn(nameof(SelectedTargetsGroup))]
        ///<summary>Включить переключатели</summary>
        public bool EnableToggleButtonChangeGroup => EnableElemetns &&
            _selectedTargetsGroup is not null
            && _selectedTargetsGroup.Year != 0;

        #endregion

        #region EnableToggleButtonChangeTarget : bool - Включить переключатель изменения цели

        [DependencyOn(nameof(SelectedTarget))]
        ///<summary>Включить переключатель изменения цели</summary>
        public bool EnableToggleButtonChangeTarget => EnableElemetns && _selectedTarget is not null;

        #endregion

        #region EnableToggleButtonCreateTarget : bool - Включить переключатель создания цели

        [DependencyOn(nameof(SelectedTargetsGroup))]
        ///<summary>Включить переключатель создания цели</summary>
        public bool EnableToggleButtonCreateTarget => EnableElemetns && _selectedTargetsGroup is not null;

        #endregion

        #region TargetsGroupForAdd_Edit : TargetsGroup - Для редактирования и добавления группы

        ///<summary>Для редактирования и добавления группы</summary>
        private TargetsGroup _targetsGroupForAdd_Edit = new();

        ///<summary>Для редактирования и добавления группы</summary>
        public TargetsGroup TargetsGroupForAdd_Edit
        {
            get => _targetsGroupForAdd_Edit;
            set => Set(ref _targetsGroupForAdd_Edit, value);
        }

        #endregion

        #region TargetForAdd_Edit : Target - Для редактирования и добавления цели

        ///<summary>Для редактирования и добавления цели</summary>
        private Target _targetForAdd_Edit = new();

        ///<summary>Для редактирования и добавления цели</summary>
        public Target TargetForAdd_Edit { get => _targetForAdd_Edit; set => Set(ref _targetForAdd_Edit, value); }

        #endregion

        #region CompletedTargetsCount : int - Количество выполненных задач

        ///<summary>Количество выполненных задач</summary>
        private int _completedTargetsCount;
        ///<summary>Количество выполненных задач</summary>
        public int CompletedTargetsCount
        {
            get => _completedTargetsCount;
            set
            {
                if (!Set(ref _completedTargetsCount, value)) return;
                PropertiesChanged(this);
            }
        }

        #endregion

        #region Progress : double - Прогресс выполнения целей

        [DependencyOn(nameof(CompletedTargetsCount))]
        [ChangesWithProperties([nameof(OffsetCompleted), nameof(Procent)])]
        ///<summary>Прогресс выполнения целей</summary>
        public double Progress => (double)CompletedTargetsCount / (_selectedTargetsGroup is not null && _selectedTargetsGroup.Targets.Count > 0
            ? _selectedTargetsGroup.Targets.Count
            : 1);

        #endregion

        #region OffsetCompleted : double - Смещение выполненных целей

        ///<summary>Смещение выполненных целей</summary>
        public double OffsetCompleted => 2.0 - Progress;

        #endregion

        #region Procent : double - Процент выполненных целей
        ///<summary>Процент выполненных целей</summary>
        public double Procent => Math.Round(Progress * 100.0, 2);

        #endregion

        #region CleanUpBehaviors : bool - Очистить поведение

        ///<summary>Очистить поведение</summary>
        private bool _cleanUpBehaviors = false;

        ///<summary>Очистить поведение</summary>
        public bool CleanUpBehaviors { get => _cleanUpBehaviors; set => Set(ref _cleanUpBehaviors, value); }

        #endregion

        #endregion

        #region Commands...

        #region LoadCommand - Загрузка пользовательского окна

        ///<summary>Загрузка пользовательского окна</summary>
        private ICommand? _loadCommand;

        ///<summary>Загрузка пользовательского окна</summary>
        public ICommand LoadCommand => _loadCommand
            ??= new LambdaCommand(OnLoadCommandExecuted);

        ///<summary>Логика выполнения - Загрузка пользовательского окна</summary>
        private void OnLoadCommandExecuted(object? p)
        {
            Targets.CollectionChanged += Targets_CollectionChanged;
            foreach (TargetsGroup targets in _targetsGroupRepository.Items)
                GroupsTargets.Add(new TargetsModel(targets));
            _selectedTargetsViewSource.Source = Targets;
        }

        #endregion

        #region ClosedCommand - Команда - закрытие пользовательского окна

        ///<summary>Команда - закрытие пользовательского окна</summary>
        private ICommand? _closedCommand;

        ///<summary>Команда - закрытие пользовательского окна</summary>
        public ICommand ClosedCommand => _closedCommand
            ??= new LambdaCommand(OnClosedCommandExecuted);

        ///<summary>Логика выполнения - закрытие пользовательского окна</summary>
        private void OnClosedCommandExecuted(object? p) => Dispose();

        #endregion

        #region CreateGroupCommand - Команда - создать новую группу

        ///<summary>Команда - создать новую группу</summary>
        private ICommand? _createTargetsGroupCommand;

        ///<summary>Команда - создать новую группу</summary>
        public ICommand CreateGroupCommand => _createTargetsGroupCommand
            ??= new LambdaCommandAsync(OnCreateGroupCommandExecuted, CanCreateGroupCommandExecute);

        private bool CanCreateGroupCommandExecute(object? p) => IsVisibleAdd_EditGroup
            && !string.IsNullOrWhiteSpace(_targetsGroupForAdd_Edit.Name);

        ///<summary>Логика выполнения - создать новую группу</summary>
        private async Task OnCreateGroupCommandExecuted(object? p)
        {
            GroupsTargets.Add(new TargetsModel(await _targetsGroupRepository.AddAsync(new TargetsGroup()
            {
                Name = _targetsGroupForAdd_Edit.Name,
                Year = _targetsGroupForAdd_Edit.Year,
            })));
            SelectedTargetsGroup = GroupsTargets.Last();
            AddGroup = false;
        }

        #endregion

        #region CancelCreateGroupCommand - Команда - отмены создания новой группы

        ///<summary>Команда - отмены создания новой группы</summary>
        private ICommand? _cancelCreateGroupCommand;

        ///<summary>Команда - отмены создания новой группы</summary>
        public ICommand CancelCreateGroupCommand => _cancelCreateGroupCommand
            ??= new LambdaCommand(OnCancelCreateGroupCommandExecuted, CanCancelCreateGroupCommandExecute);

        private bool CanCancelCreateGroupCommandExecute(object? p) => IsVisibleAdd_EditGroup;

        ///<summary>Логика выполнения - отмены создания новой группы</summary>
        private void OnCancelCreateGroupCommandExecuted(object? p) => AddGroup = ChangeGroup = false;

        #endregion

        #region ChangeGroupCommand - Команда - изменить значения группы

        ///<summary>Команда - изменить значения группы</summary>
        private ICommand? _changeGroupCommand;

        ///<summary>Команда - изменить значения группы</summary>
        public ICommand ChangeGroupCommand => _changeGroupCommand
            ??= new LambdaCommandAsync(OnChangeGroupCommandExecuted, CanChangeGroupCommandExecute);

        ///<summary>Проверка возможности выполнения - изменить значения группы</summary>
        private bool CanChangeGroupCommandExecute(object? p) => IsVisibleAdd_EditGroup
            && _selectedTargetsGroup is not null
            && !string.IsNullOrWhiteSpace(_targetsGroupForAdd_Edit.Name)
            ;

        ///<summary>Логика выполнения - изменить значения группы</summary>
        private async Task OnChangeGroupCommandExecuted(object? p)
        {
            _selectedTargetsGroup!.Year = _targetsGroupForAdd_Edit.Year;
            _selectedTargetsGroup.Name = _targetsGroupForAdd_Edit.Name;
            var item = await _targetsGroupRepository.GetAsync(_selectedTargetsGroup.Id);
            await _targetsGroupRepository.UpdateAsync(item);
            CollectionViewSource.GetDefaultView(GroupsTargets).Refresh();
            ChangeGroup = false;
        }

        #endregion

        #region RemoveTargetsGroupCommand - Команда - удалить группу

        ///<summary>Команда - удалить группу</summary>
        private ICommand? _removeTargetsGroupCommand;

        ///<summary>Команда - удалить группу</summary>
        public ICommand RemoveTargetsGroupCommand => _removeTargetsGroupCommand
            ??= new LambdaCommandAsync(OnRemoveTargetsGroupCommandExecuted, CanRemoveTargetsGroupCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить группу</summary>
        private bool CanRemoveTargetsGroupCommandExecute(object? p) => _selectedTargetsGroup is not null
            && _selectedTargetsGroup.Year != 0
            && !IsVisibleAdd_EditGroup
            && !IsVisibleAdd_EditTarget
            ;

        ///<summary>Логика выполнения - удалить группу</summary>
        private async Task OnRemoveTargetsGroupCommandExecuted(object? p)
        {
            ClearTargets();
            await _targetsGroupRepository.RemoveAsync(_selectedTargetsGroup!.Id);
            GroupsTargets.Remove(_selectedTargetsGroup);
            SelectedTargetsGroup = GroupsTargets.Count > 0 ? GroupsTargets.Last() : null;
        }

        #endregion

        #region CreateTargetCommand - Команда - создать новую цель

        ///<summary>Команда - создать новую цель</summary>
        private ICommand? _createTargetCommand;

        ///<summary>Команда - создать новую цель</summary>
        public ICommand CreateTargetCommand => _createTargetCommand
            ??= new LambdaCommandAsync(OnCreateTargetCommandExecuted, CanCreateTargetCommandExecute);

        ///<summary>Проверка возможности выполнения - создать новую цель</summary>
        private bool CanCreateTargetCommandExecute(object? p) => IsVisibleAdd_EditTarget
            && !string.IsNullOrWhiteSpace(_targetForAdd_Edit.Name)
            ;

        ///<summary>Логика выполнения - создать новую цель</summary>
        private async Task OnCreateTargetCommandExecuted(object? p)
        {
            var newTarget = new Target()
            {
                Name = _targetForAdd_Edit.Name,
                Note = _targetForAdd_Edit.Note,
                IsComplete = _targetForAdd_Edit.IsComplete,
                TargetsGroupId = _selectedTargetsGroup.Id,
            };
            var newTargetModel = new TargetModel(newTarget);
            SelectedTargetsGroup?.Targets.Add(newTargetModel);
            if (_filter != "выполненные") Targets.Add(newTargetModel);
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(OffsetCompleted));
            OnPropertyChanged(nameof(Procent));
            await _targetRepository.AddAsync(newTarget);
            AddTarget = false;
            SelectedTargetsView.Refresh();
        }

        #endregion

        #region ChangeTargetCommand - Команда - изменить цель

        ///<summary>Команда - изменить цель</summary>
        private ICommand? _changeTargetCommand;

        ///<summary>Команда - изменить цель</summary>
        public ICommand ChangeTargetCommand => _changeTargetCommand
            ??= new LambdaCommandAsync(OnChangeTargetCommandExecuted, CanChangeTargetCommandExecute);

        ///<summary>Проверка возможности выполнения - изменить цель</summary>
        private bool CanChangeTargetCommandExecute(object? p) => IsVisibleAdd_EditTarget
            && _selectedTarget is not null
            && !string.IsNullOrWhiteSpace(_targetForAdd_Edit.Name)
            && (_targetForAdd_Edit.Name != _selectedTarget.Name
            || _targetForAdd_Edit.Note != _selectedTarget.Note)
            ;

        ///<summary>Логика выполнения - изменить цель</summary>
        private async Task OnChangeTargetCommandExecuted(object? p)
        {
            _selectedTarget!.Name = _targetForAdd_Edit.Name;
            _selectedTarget.Note = _targetForAdd_Edit.Note;
            await _targetRepository.UpdateAsync(await _targetRepository.GetAsync(_selectedTarget.Id));
            ChangeTarget = false;
            SelectedTargetsView.Refresh();
        }

        #endregion

        #region CancelCreate_EditTargetCommand - Команда - отмены создания и редактирования цели

        ///<summary>Команда - отмены создания и редактирования цели</summary>
        private ICommand? _cancelCreate_EditCommandCommand;

        ///<summary>Команда - отмены создания и редактирования цели</summary>
        public ICommand CancelCreate_EditTargetCommand => _cancelCreate_EditCommandCommand
            ??= new LambdaCommand(OnCancelCreate_EditTargetCommandExecuted, CanCancelCreate_EditTargetCommandExecute);

        ///<summary>Проверка возможности выполнения - отмены создания и редактирования цели</summary>
        private bool CanCancelCreate_EditTargetCommandExecute(object? p) => IsVisibleAdd_EditTarget;

        ///<summary>Логика выполнения - отмены создания и редактирования цели</summary>
        private void OnCancelCreate_EditTargetCommandExecuted(object? p)
            => AddTarget = ChangeTarget = false;

        #endregion

        #region RemoveTargetCommand - Команда - удалить цель

        ///<summary>Команда - удалить цель</summary>
        private ICommand? _removeTargetCommand;

        ///<summary>Команда - удалить цель</summary>
        public ICommand RemoveTargetCommand => _removeTargetCommand
            ??= new LambdaCommandAsync(OnRemoveTargetCommandExecuted, CanRemoveTargetCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить цель</summary>
        private bool CanRemoveTargetCommandExecute(object? p) => !IsVisibleAdd_EditTarget
            && _selectedTarget is not null
            ;

        ///<summary>Логика выполнения - удалить цель</summary>
        private async Task OnRemoveTargetCommandExecuted(object? p)
        {
            int index = Targets.IndexOf(_selectedTarget);
            bool isComplete = _selectedTarget.IsComplete;
            
            await _targetRepository.RemoveAsync(_selectedTarget.Id);
            SelectedTargetsGroup?.Targets.RemoveAt(index);
            Targets.RemoveAt(index);
            if (isComplete) CompletedTargetsCount--;
            else
            {
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(OffsetCompleted));
                OnPropertyChanged(nameof(Procent));
            }
        }

        #endregion

        #region SaveRepositoryCommand - Команда - Сохранить весь репозиторий

        ///<summary>Команда - Сохранить весь репозиторий</summary>
        private ICommand? _saveRepositoryCommand;

        ///<summary>Команда - Сохранить весь репозиторий</summary>
        public ICommand SaveRepositoryCommand => _saveRepositoryCommand
            ??= new LambdaCommandAsync(OnSaveRepositoryCommandExecuted, CanSaveRepositoryCommandExecute);

        ///<summary>Проверка возможности выполнения - Сохранить весь репозиторий</summary>
        private bool CanSaveRepositoryCommandExecute(object? p) =>
            _targetsGroupRepository is not null
            && !_targetsGroupRepository.AutoSaveChanges;

        ///<summary>Логика выполнения - Сохранить весь репозиторий</summary>
        private async Task OnSaveRepositoryCommandExecuted(object? p) => await _targetsGroupRepository.SaveChangedAsync();

        #endregion

        #region FilterCommand - Команда - фильтровать список

        ///<summary>Команда - фильтровать список</summary>
        private ICommand? _filterCommand;

        ///<summary>Команда - фильтровать список</summary>
        public ICommand FilterCommand => _filterCommand
            ??= new LambdaCommand<string>(OnFilterCommandExecuted, CanFilterCommandExecute);

        ///<summary>Проверка возможности выполнения - фильтровать список</summary>
        private bool CanFilterCommandExecute(string p) => _selectedTargetsGroup is not null;

        ///<summary>Логика выполнения - фильтровать список</summary>
        private void OnFilterCommandExecuted(string p)
        {
            _filter = p.ToLower();
            ClearTargets();
            switch (_filter)
            {
                case "все":
                    foreach (var target in _selectedTargetsGroup.Targets)
                        Targets.Add(target);
                    break;
                case "выполненные":
                    foreach (var target in _selectedTargetsGroup.Targets.Where(t => t.IsComplete))
                        Targets.Add(target);
                    break;
                case "невыполненные":
                    foreach (var target in _selectedTargetsGroup.Targets.Where(t => !t.IsComplete))
                        Targets.Add(target);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #endregion

        #region Methods...

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                CleanUpBehaviors = true;
                Targets.CollectionChanged -= Targets_CollectionChanged;
                ClearTargets();
                GroupsTargets.Clear();

                if (disposing)
                {
                }
                _disposed = true;
            }
        }

        private void ClearTargets(int callCount = 0)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => ClearTargets(callCount));
                return;
            }

            for (int i = 0; i < Targets.Count; i++)
                Targets[i].PropertyChanged -= Target_PropertyChanged;

            Targets.Clear();
        }

        #endregion

        #region Events...

        private void Targets_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null && e.NewItems[0] is TargetModel newTarget)
                    {
                        if (newTarget.IsComplete) _completedTargetsCount_Calculated++;
                        newTarget.PropertyChanged += Target_PropertyChanged;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null && e.OldItems[0] is TargetModel oldTarget)
                    {
                        oldTarget.PropertyChanged -= Target_PropertyChanged;
                        _completedTargetsCount_Calculated--;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    _completedTargetsCount_Calculated = 0;
                    break;
                default:
                    throw new NotImplementedException("Данная функция не реализована!");
            }
        }

        private void Target_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is TargetModel target && e.PropertyName == nameof(target.IsComplete))
            {
                CompletedTargetsCount += target.IsComplete ? 1 : -1;
                _targetRepository.Update(_targetRepository.Get(target.Id));
            }
        }

        #endregion
    }
}
