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
                                  IRepository<TargetsGroup> targetsRepository) : ViewModel
    {
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IRepository<TargetsGroup> _targetsRepository = targetsRepository;

        #region Properties...

        #region GroupsTargets : ObservableCollection<TargetsModel> - Список групп с целями

        ///<summary>Список групп с целями</summary>
        public ObservableCollection<TargetsModel> GroupsTargets { get; } = [];

        #endregion

        #region SelectedTargets : TargetsModel - Выбранный список целей

        ///<summary>Выбранный список целей</summary>
        private TargetsModel? _selectedTargets;

        ///<summary>Выбранный список целей</summary>
        public TargetsModel? SelectedTargets
        {
            get => _selectedTargets;
            set
            {
                if (_selectedTargets == value) return;
                _selectedTargets?.Targets.Clear();
                if (value is not null && value.Targets.Count == 0)
                {
                    var targets = _targetsRepository.Items.Include(g => g.Targets).FirstOrDefault(ts => ts.Id == value.Id)?.Targets;
                    if (targets is not null)
                    {
                        foreach (var target in targets)
                            value.Targets.Add(new TargetModel(target));
                    }
                }
                Set(ref _selectedTargets, value);
                DepedenciesChanged();

            }
        }

        #endregion

        private readonly CollectionViewSource _selectedTargetsViewSource = new();

        [DependencyOn(nameof(SelectedTargets))]
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
        [PropertyChangedWith(nameof(EnableToggleButtonChangeGroup))]
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

        #region ChangeGroup : bool - Изменить группу

        ///<summary>Изменить группу</summary>
        private bool _changeGroup;

        [PropertyChangedWith(nameof(IsVisibleAdd_EditGroup))]
        [PropertyChangedWith(nameof(EnableToggleButtons))]
        [PropertyChangedWith(nameof(EnableToggleButtonChangeGroup))]
        ///<summary>Изменить группу</summary>
        public bool ChangeGroup
        {
            get => _changeGroup;
            set
            {
                if (!Set(ref _changeGroup, value)) return;
                if (value && _selectedTargets is not null)
                {
                    TargetsGroupForAdd_Edit.Year = _selectedTargets.Year;
                    TargetsGroupForAdd_Edit.Name = _selectedTargets.Name;
                    OnPropertyChanged(nameof(TargetsGroupForAdd_Edit));
                }
                ChangedWithProperties();
            }
        }

        #endregion

        #region IsVisibleAdd_EditGroup : bool - Видимость окна создания или редактирования группы

        ///<summary>Видимость окна создания или редактирования группы</summary>
        public bool IsVisibleAdd_EditGroup => _addGroup || _changeGroup;

        #endregion

        #region EnableToggleButtons : bool - Включить переключатели

        ///<summary>Включить переключатели</summary>
        public bool EnableToggleButtons => !IsVisibleAdd_EditGroup;

        #endregion

        #region EnableToggleButtonChangeGroup : bool - Включить переключатель изменения групп

        [DependencyOn(nameof(SelectedTargets))]
        ///<summary>Включить переключатели</summary>
        public bool EnableToggleButtonChangeGroup => !IsVisibleAdd_EditGroup && _selectedTargets?.Year != 0;

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
            _selectedTargetsViewSource.Source = SelectedTargets?.Targets;
            OnPropertyChanged(nameof(SelectedTargetsView));
        }
        #endregion

        #region CreateGroupCommand - Команда - создать новую группу

        ///<summary>Команда - создать новую группу</summary>
        private ICommand? _createTargetsGroupCommand;

        ///<summary>Команда - создать новую группу</summary>
        public ICommand CreateGroupCommand => _createTargetsGroupCommand
            ??= new LambdaCommandAsync(OnCreateGroupCommandExecuted, CanCreateGroupCommandExecute);

        private bool CanCreateGroupCommandExecute(object? p) => !string.IsNullOrWhiteSpace(_targetsGroupForAdd_Edit.Name);

        ///<summary>Логика выполнения - создать новую группу</summary>
        private async Task OnCreateGroupCommandExecuted(object? p)
        {
            GroupsTargets.Add(new TargetsModel(await _targetsRepository.AddAsync(new TargetsGroup()
            {
                Name = _targetsGroupForAdd_Edit.Name,
                Year = _targetsGroupForAdd_Edit.Year,
            })));
            AddGroup = false;
        }

        #endregion

        #region CancelCreateGroupCommand - Команда - отмены создания новой группы

        ///<summary>Команда - отмены создания новой группы</summary>
        private ICommand? _cancelCreateGroupCommand;

        ///<summary>Команда - отмены создания новой группы</summary>
        public ICommand CancelCreateGroupCommand => _cancelCreateGroupCommand
            ??= new LambdaCommand(OnCancelCreateGroupCommandExecuted);

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
        private bool CanChangeGroupCommandExecute(object? p) => _selectedTargets is not null;

        ///<summary>Логика выполнения - изменить значения группы</summary>
        private async Task OnChangeGroupCommandExecuted(object? p)
        {
            _selectedTargets!.Year = _targetsGroupForAdd_Edit.Year;
            _selectedTargets.Name = _targetsGroupForAdd_Edit.Name;
            var item = _targetsRepository.Get(_selectedTargets.Id);
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
        private bool CanRemoveTargetsGroupCommandExecute(object? p) => _selectedTargets is not null
            && _selectedTargets.Year != 0
            && !IsVisibleAdd_EditGroup
            ;

        ///<summary>Логика выполнения - удалить группу</summary>
        private async Task OnRemoveTargetsGroupCommandExecuted(object? p)
        {
            await _targetsRepository.RemoveAsync(_selectedTargets!.Id);
            GroupsTargets.Remove(_selectedTargets);
            SelectedTargets = GroupsTargets.Count > 0 ? GroupsTargets.Last() : null;
        }

        #endregion

        #region FilterCommand - Команда - фильтровать список

        ///<summary>Команда - фильтровать список</summary>
        private ICommand? _filterCommand;

        ///<summary>Команда - фильтровать список</summary>
        public ICommand FilterCommand => _filterCommand
            ??= new LambdaCommand<string>(OnFilterCommandExecuted, CanFilterCommandExecute);

        ///<summary>Проверка возможности выполнения - фильтровать список</summary>
        private bool CanFilterCommandExecute(string p) => _selectedTargets is not null;

        ///<summary>Логика выполнения - фильтровать список</summary>
        private void OnFilterCommandExecuted(string p)
        {
            _selectedTargets!.Targets.Clear();
            switch (p.ToLower())
            {
                case "все":
                    foreach (var target in _targetsRepository.Items
                        .Include(g => g.Targets)
                        .First(ts => ts.Id == _selectedTargets.Id)
                        .Targets)
                        _selectedTargets.Targets.Add(new TargetModel(target));
                    break;
                case "выполненные":
                    foreach (var target in _targetsRepository.Items
                        .Include(g => g.Targets)
                        .First(ts => ts.Id == _selectedTargets.Id)
                        .Targets.Where(t => t.IsComplete))
                        _selectedTargets.Targets.Add(new TargetModel(target));
                    break;
                case "невыполненные":
                    foreach (var target in _targetsRepository.Items
                        .Include(g => g.Targets)
                        .First(ts => ts.Id == _selectedTargets.Id)
                        .Targets.Where(t => !t.IsComplete))
                        _selectedTargets.Targets.Add(new TargetModel(target));
                    break;
                default:
                    break;
            }
        }

        #endregion

        #endregion
        public TargetsUCViewModel() : this(null, null, null)
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
                if(attributes is not null)
                    foreach (var attribute in attributes)
                        OnPropertyChanged(attribute.PropertyName);
            }
        }
    }
}
