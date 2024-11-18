using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Attributes;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Models.Targets;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class TargetsUCViewModel(IOpenWindows openWindows,
                                  IUserDialog userDialog,
                                  IRepository<Target> targetRepository,
                                  IRepository<TargetsGroup> targetsGroupRepository) : ViewModel, IDisposable
    {
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IRepository<Target> _targetRepository = targetRepository;
        private readonly IRepository<TargetsGroup> _targetsRepository = targetsGroupRepository;
        private bool _disposed = false;

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
                if (value is not null)
                {
                    if (value.Targets.Count == 0)
                    {
                        var targets = _targetsRepository.Items.Include(g => g.Targets).FirstOrDefault(ts => ts.Id == value.Id)?.Targets;
                        if (targets is not null)
                        {
                            foreach (var target in targets)
                                value.Targets.Add(new TargetModel(target));
                        }
                    }
                }
                Set(ref _selectedTargetsGroup, value);
                _selectedTargetsViewSource.Source = _selectedTargetsGroup?.Targets;
                DepedenciesChanged();

            }
        }

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
                DepedenciesChanged();
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
                    _selectedTargetsViewSource.SortDescriptions.RemoveAt(0);
                _selectedTargetsViewSource.SortDescriptions.Add(Sorts[value.Key]);
                _selectedTargetsViewSource.View.Refresh();
            }
        }

        #endregion

        #region AutoSave : bool - Автосохранение

        ///<summary>Автосохранение</summary>
        public bool AutoSave
        {
            get => _targetsRepository.AutoSaveChanges;
            set
            {
                if (_targetsRepository.AutoSaveChanges == value) return;
                _targetsRepository.AutoSaveChanges = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region AddGroup : bool - Добавить группу

        ///<summary>Добавить группу</summary>
        private bool _addGroup;

        [PropertyChangedWith(nameof(IsVisibleAdd_EditGroup))]
        [PropertyChangedWith(nameof(EnableToggleButtons))]
        [PropertyChangedWith(nameof(EnableToggleButtonCreateTarget))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeGroup))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeTarget))]
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
                ChangedWithProperties();
            }
        }

        #endregion

        #region AddTarget : bool - Добавить цель

        ///<summary>Добавить цель</summary>
        private bool _addTarget;

        [PropertyChangedWith(nameof(IsVisibleAdd_EditTarget))]
        [PropertyChangedWith(nameof(EnableToggleButtons))]
        [PropertyChangedWith(nameof(EnableToggleButtonCreateTarget))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeGroup))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeTarget))]
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
                ChangedWithProperties();
            }
        }

        #endregion

        #region ChangeGroup : bool - Изменить группу

        ///<summary>Изменить группу</summary>
        private bool _changeGroup;

        [PropertyChangedWith(nameof(IsVisibleAdd_EditGroup))]
        [PropertyChangedWith(nameof(EnableToggleButtons))]
        [PropertyChangedWith(nameof(EnableToggleButtonCreateTarget))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeGroup))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeTarget))]
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
                ChangedWithProperties();
            }
        }

        #endregion

        #region ChangeTarget : bool - Изменить цель

        ///<summary>Изменить цель</summary>
        private bool _changeTarget;

        [PropertyChangedWith(nameof(IsVisibleAdd_EditTarget))]
        [PropertyChangedWith(nameof(EnableToggleButtons))]
        [PropertyChangedWith(nameof(EnableToggleButtonCreateTarget))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeGroup))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeTarget))]
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
                    TargetForAdd_Edit.IsComplete = _selectedTarget.IsComplete;
                    OnPropertyChanged(nameof(TargetForAdd_Edit));
                }
                ChangedWithProperties();
            }
        }

        #endregion

        #region IsVisibleAdd_EditGroup : bool - Видимость окна создания или редактирования группы

        ///<summary>Видимость окна создания или редактирования группы</summary>
        public bool IsVisibleAdd_EditGroup => _addGroup || _changeGroup;

        #endregion

        #region IsVisibleAdd_EditTarget : bool - Видимость окна создания или редактирования цели

        ///<summary>Видимость окна создания или редактирования цели</summary>
        public bool IsVisibleAdd_EditTarget => _addTarget || _changeTarget;

        #endregion

        #region EnableToggleButtons : bool - Включить переключатели

        ///<summary>Включить переключатели</summary>
        public bool EnableToggleButtons => !IsVisibleAdd_EditGroup && !IsVisibleAdd_EditTarget;

        #endregion

        #region EnableToggleButtonChangeGroup : bool - Включить переключатель изменения групп

        [DependencyOn(nameof(SelectedTargetsGroup))]
        ///<summary>Включить переключатели</summary>
        public bool EnableToggleButtonChangeGroup => EnableToggleButtons &&
            _selectedTargetsGroup is not null
            && _selectedTargetsGroup.Year != 0;

        #endregion

        #region EnableToggleButtonChangeTarget : bool - Включить переключатель изменения цели

        [DependencyOn(nameof(SelectedTarget))]
        ///<summary>Включить переключатель изменения цели</summary>
        public bool EnableToggleButtonChangeTarget => EnableToggleButtons && _selectedTarget is not null;

        #endregion

        #region EnableToggleButtonCreateTarget : bool - Включить переключатель создания цели

        [DependencyOn(nameof(SelectedTargetsGroup))]
        ///<summary>Включить переключатель создания цели</summary>
        public bool EnableToggleButtonCreateTarget => EnableToggleButtons && _selectedTargetsGroup is not null;

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

        #endregion

        #region Commands...

        #region LoadCommand - Загрузка окна

        ///<summary>Загрузка окна</summary>
        private ICommand? _loadCommand;

        ///<summary>Загрузка окна</summary>
        public ICommand LoadCommand => _loadCommand
            ??= new LambdaCommand(OnLoadCommandExecuted);

        ///<summary>Логика выполнения - Загрузка окна</summary>
        private void OnLoadCommandExecuted(object? p)
        {
            GroupsTargets.Clear();
            foreach (TargetsGroup targets in _targetsRepository.Items)
                GroupsTargets.Add(new TargetsModel(targets));
        }
        #endregion

        #region ClosedCommand - Команда - закрытие окна

        ///<summary>Команда - закрытие окна</summary>
        private ICommand? _closedCommand;

        ///<summary>Команда - закрытие окна</summary>
        public ICommand ClosedCommand => _closedCommand
            ??= new LambdaCommand(OnClosedCommandExecuted, CanClosedCommandExecute);

        ///<summary>Проверка возможности выполнения - закрытие окна</summary>
        private bool CanClosedCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - закрытие окна</summary>
        private void OnClosedCommandExecuted(object? p)
        {
            Dispose();
            GroupsTargets.Clear();
        }

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
            GroupsTargets.Add(new TargetsModel(await _targetsRepository.AddAsync(new TargetsGroup()
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
            var item = await _targetsRepository.GetAsync(_selectedTargetsGroup.Id);
            await _targetsRepository.UpdateAsync(item);
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
            ;

        ///<summary>Логика выполнения - удалить группу</summary>
        private async Task OnRemoveTargetsGroupCommandExecuted(object? p)
        {
            await _targetsRepository.RemoveAsync(_selectedTargetsGroup!.Id);
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
            SelectedTargetsGroup?.Targets.Add(new TargetModel(newTarget));
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
            ;

        ///<summary>Логика выполнения - изменить цель</summary>
        private async Task OnChangeTargetCommandExecuted(object? p)
        {
            _selectedTarget!.Name = _targetForAdd_Edit.Name;
            _selectedTarget.Note = _targetForAdd_Edit.Note;
            _selectedTarget.IsComplete = _targetForAdd_Edit.IsComplete;
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
            await _targetRepository.RemoveAsync(_selectedTarget.Id);
            SelectedTargetsGroup?.Targets.Remove(_selectedTarget);
        }

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
            SelectedTargetsGroup!.Targets.Clear();
            switch (p.ToLower())
            {
                case "все":
                    foreach (var target in _targetsRepository.Items
                        .Include(g => g.Targets)
                        .First(ts => ts.Id == SelectedTargetsGroup.Id)
                        .Targets)
                        SelectedTargetsGroup.Targets.Add(new TargetModel(target));
                    break;
                case "выполненные":
                    foreach (var target in _targetsRepository.Items
                        .Include(g => g.Targets)
                        .First(ts => ts.Id == SelectedTargetsGroup.Id)
                        .Targets.Where(t => t.IsComplete))
                        SelectedTargetsGroup.Targets.Add(new TargetModel(target));
                    break;
                case "невыполненные":
                    foreach (var target in _targetsRepository.Items
                        .Include(g => g.Targets)
                        .First(ts => ts.Id == SelectedTargetsGroup.Id)
                        .Targets.Where(t => !t.IsComplete))
                        SelectedTargetsGroup.Targets.Add(new TargetModel(target));
                    break;
                default:
                    break;
            }
        }

        #endregion

        #endregion

        public TargetsUCViewModel() : this(null, null, null, null)
        {

        }

        private void DepedenciesChanged([CallerMemberName] string? propertyName = null)
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                var depedencyAttribute = property.GetCustomAttribute<DependencyOnAttribute>();
                if (depedencyAttribute != null && depedencyAttribute.PropertyName == propertyName)
                    OnPropertyChanged(property.Name);
            }
        }

        private void ChangedWithProperties([CallerMemberName] string? propertyName = null)
        {
            var property = GetType().GetProperty(propertyName!);
            if (property is not null)
            {
                var attributes = property.GetCustomAttributes<PropertyChangedWithAttribute>();
                if (attributes is not null)
                    foreach (var attribute in attributes)
                        OnPropertyChanged(attribute.PropertyName);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var targetsGroup in GroupsTargets)
                    {
                        targetsGroup.Dispose();
                    }
                }
                _disposed = true;
            }
        }
    }
}
