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
    internal sealed class TargetsUCViewModel(IRepository<Target> targetRepository,
                             IRepository<TargetsGroup> targetsGroupRepository) : MainFunctionsViewModel<TargetsGroup>(targetsGroupRepository), IDisposable
    {
        private readonly IRepository<Target> _targetRepository = targetRepository;
        private bool _disposed = false;
        private string _filter = "все";
        private int _completedTargetsCount_Calculated = 0;

        #region Properties...

        #region override EnableFrameworkElements - Включить визуальные элементы

        [DependencyOn(nameof(ShowAdd_EditTargetUserControl))]
        [IsMoveToTree]
        public override bool EnableFrameworkElements => base.EnableFrameworkElements && !ShowAdd_EditTargetUserControl;

        #endregion

        #region TargetsCount : int - Количество целей (в общем)

        [DependencyOn(nameof(SelectedTargetsGroup))]
        ///<summary>Количество целей (в общем)</summary>
        public int TargetsCount => _selectedTargetsGroup?.Targets.Count ?? 0;

        #endregion

        #region GroupsTargets : ObservableCollection<TargetsModel> - Список групп с целями

        ///<summary>Список групп с целями</summary>
        public ObservableCollection<TargetsGroup> GroupsTargets { get; } = [];

        #endregion

        #region SelectedTargetsGroup : TargetsModel - Выбранная группа

        ///<summary>Выбранная группа</summary>
        private TargetsGroup? _selectedTargetsGroup;

        ///<summary>Выбранная группа</summary>
        public TargetsGroup? SelectedTargetsGroup
        {
            get => _selectedTargetsGroup;
            set
            {
                if (_selectedTargetsGroup == value) return;
                ClearTargets();
                if (value is not null)
                {
                    if (value.Targets.Count == 0)
                        _ = _itemsRepository.Items.Include(g => g.Targets).FirstOrDefault(ts => ts.Id == value.Id);
                    foreach (var target in value.Targets)
                        Targets.Add(new TargetModel(target));
                }
                Set(ref _selectedTargetsGroup, value);
                CompletedTargetsCount = _completedTargetsCount_Calculated;
                DepedencyProperites(this);

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
                DepedencyProperites(this);
            }
        }

        #endregion

        private readonly CollectionViewSource _selectedTargetsViewSource = new();

        [DependencyOn(nameof(SelectedTargetsGroup))]
        public ICollectionView SelectedTargetsView => _selectedTargetsViewSource.View;

        #region Sort : Dictionary<string, SortDescription> - Сортировка

        /// <summary> Сортировка </summary>
        public Dictionary<string, SortDescription> Sort { get; } = new()
        {
            {"Сначала старые", new SortDescription("Id", ListSortDirection.Ascending)},
            {"Сначала свежие", new SortDescription("Id", ListSortDirection.Descending)},
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
                    _selectedTargetsViewSource.SortDescriptions[0] = Sort[value.Key];
                else _selectedTargetsViewSource.SortDescriptions.Add(Sort[value.Key]);
                _selectedTargetsViewSource.View?.Refresh();
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
                DepedencyProperites(this);
            }
        }

        #endregion

        #region EditTarget : bool - Изменить цель

        ///<summary>Изменить цель</summary>
        private bool _editTarget;
        ///<summary>Изменить цель</summary>
        public bool EditTarget
        {
            get => _editTarget;
            set
            {
                if (!Set(ref _editTarget, value)) return;
                if (value && _selectedTarget is not null)
                {
                    TargetForAdd_Edit.Name = _selectedTarget.Name;
                    TargetForAdd_Edit.Note = _selectedTarget.Note;
                    OnPropertyChanged(nameof(TargetForAdd_Edit));
                }
                DepedencyProperites(this);
            }
        }

        #endregion

        #region ShowAdd_EditTargetUserControl : bool - Видимость окна создания или редактирования цели

        [DependencyOn([nameof(AddTarget), nameof(EditTarget)])]
        [IsMoveToTree]
        ///<summary>Видимость окна создания или редактирования цели</summary>
        public bool ShowAdd_EditTargetUserControl => _addTarget || _editTarget;

        #endregion

        #region EnableToggleButtonEditGroup : bool - Включить переключатель изменения группы целей

        [DependencyOn([nameof(EnableFrameworkElements), nameof(SelectedTargetsGroup)])]
        ///<summary> Включить переключатель изменения группы целей </summary>
        public bool EnableToggleButtonEditGroup => EnableFrameworkElements &&
            _selectedTargetsGroup is not null
            && _selectedTargetsGroup.Year != 0;

        #endregion

        #region EnableToggleButtonEditTarget : bool - Включить переключатель изменения цели

        [DependencyOn([nameof(EnableFrameworkElements), nameof(SelectedTarget)])]
        ///<summary>Включить переключатель изменения цели</summary>
        public bool EnableToggleButtonEditTarget => EnableFrameworkElements && _selectedTarget is not null;

        #endregion

        #region EnableToggleButtonAddTarget : bool - Включить переключатель добавления цели

        [DependencyOn([nameof(EnableFrameworkElements), nameof(SelectedTargetsGroup)])]
        ///<summary>Включить переключатель добавления цели</summary>
        public bool EnableToggleButtonAddTarget => EnableFrameworkElements && _selectedTargetsGroup is not null;

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
                DepedencyProperites(this);
            }
        }

        #endregion

        #region Progress : double - Прогресс выполнения целей

        [DependencyOn(nameof(CompletedTargetsCount))]
        [IsMoveToTree]
        ///<summary>Прогресс выполнения целей</summary>
        public double Progress => (double)CompletedTargetsCount / (TargetsCount > 0
            ? TargetsCount
            : 1);

        #endregion

        #region OffsetCompleted : double - Смещение выполненных целей
        [DependencyOn(nameof(Progress))]
        ///<summary>Смещение выполненных целей</summary>
        public double OffsetCompleted => 2.0 - Progress;

        #endregion

        #region Procent : double - Процент выполненных целей
        [DependencyOn(nameof(Progress))]
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

        #region override LoadedCommand - Команда - Загрузка окна

        protected override void OnLoadedCommandExecuted(object? p)
        {
            Targets.CollectionChanged += Targets_CollectionChanged;
            foreach (TargetsGroup targets in _itemsRepository.Items)
                GroupsTargets.Add(targets);
            _selectedTargetsViewSource.Source = Targets;
        }

        #endregion

        #region override ClosedCommand - Команда - закрытие пользовательского окна

        protected override void OnClosedCommandExecuted(object? p) => Dispose();

        #endregion

        #region override AddElementCommand - Команда - добавить группу целей

        protected override bool CanAddElementCommandExecute(object? p) => ShowAdd_EditUserControl
            && !string.IsNullOrWhiteSpace(_targetsGroupForAdd_Edit.Name);

        protected override async Task OnAddElementCommandExecuted(object? p)
        {
            GroupsTargets.Add(await _itemsRepository.AddAsync(new TargetsGroup()
            {
                Name = _targetsGroupForAdd_Edit.Name,
                Year = _targetsGroupForAdd_Edit.Year,
            }));
            AddElement = false;
            SelectedTargetsGroup = GroupsTargets.Last();
        }

        #endregion

        #region override EditElementCommand - Команда - редактировать группу целей

        protected override bool CanEditElementCommandExecute(object? p) => ShowAdd_EditUserControl
            && _selectedTargetsGroup is not null
            && !string.IsNullOrWhiteSpace(_targetsGroupForAdd_Edit.Name)
            ;

        protected override async Task OnEditElementCommandExecuted(object? p)
        {
            _selectedTargetsGroup!.Year = _targetsGroupForAdd_Edit.Year;
            _selectedTargetsGroup.Name = _targetsGroupForAdd_Edit.Name;
            await _itemsRepository.UpdateAsync(await _itemsRepository.GetAsync(_selectedTargetsGroup.Id));
            CollectionViewSource.GetDefaultView(GroupsTargets).Refresh();
            EditElement = false;
        }

        #endregion

        #region override DeleteElementCommand - Команда - удалить группу целей

        protected override bool CanDeleteElementCommandExecute(object? p) => _selectedTargetsGroup is not null
            && _selectedTargetsGroup.Year != 0
            && !ShowAdd_EditUserControl
            && !ShowAdd_EditTargetUserControl
            ;

        protected override async Task OnDeleteElementCommandExecuted(object? p)
        {
            ClearTargets();
            await _itemsRepository.RemoveAsync(_selectedTargetsGroup!.Id);
            GroupsTargets.Remove(_selectedTargetsGroup);
            SelectedTargetsGroup = GroupsTargets.Count > 0 ? GroupsTargets.Last() : null;
        }

        #endregion

        #region override CancelOperationCommand - Команда - отменить операцию

        protected override bool CanCancelOperationCommandExecute(object? p) =>
            base.CanCancelOperationCommandExecute(p)
            || ShowAdd_EditTargetUserControl
            ;

        protected override void OnCancelOperationCommandExecuted(object? p)
        {
            base.OnCancelOperationCommandExecuted(p);
            AddTarget = EditTarget = false;
        }

        #endregion

        #region AddTargetCommand - Команда - создать новую цель

        ///<summary>Команда - создать новую цель</summary>
        private ICommand? _addTargetCommand;

        ///<summary>Команда - создать новую цель</summary>
        public ICommand AddTargetCommand => _addTargetCommand
            ??= new LambdaCommandAsync(OnAddTargetCommandExecuted, CanAddTargetCommandExecute);

        ///<summary>Проверка возможности выполнения - создать новую цель</summary>
        private bool CanAddTargetCommandExecute(object? p) => ShowAdd_EditTargetUserControl
            && !string.IsNullOrWhiteSpace(_targetForAdd_Edit.Name)
            ;

        ///<summary>Логика выполнения - создать новую цель</summary>
        private async Task OnAddTargetCommandExecuted(object? p)
        {
            var newTarget = new Target()
            {
                Name = _targetForAdd_Edit.Name,
                Note = _targetForAdd_Edit.Note,
                IsComplete = _targetForAdd_Edit.IsComplete,
                TargetsGroupId = _selectedTargetsGroup.Id,
            };
            await _targetRepository.AddAsync(newTarget);
            var newTargetModel = new TargetModel(newTarget);
            if (_filter != "выполненные") Targets.Add(newTargetModel);
            OnPropertyChanged(nameof(TargetsCount));
            DepedencyProperites(this, nameof(CompletedTargetsCount));
            AddTarget = false;
        }

        #endregion

        #region EditTargetCommand - Команда - изменить цель

        ///<summary>Команда - изменить цель</summary>
        private ICommand? _editTargetCommand;

        ///<summary>Команда - изменить цель</summary>
        public ICommand EditTargetCommand => _editTargetCommand
            ??= new LambdaCommandAsync(OnEditTargetCommandExecuted, CanEditTargetCommandExecute);

        ///<summary>Проверка возможности выполнения - изменить цель</summary>
        private bool CanEditTargetCommandExecute(object? p) => ShowAdd_EditTargetUserControl
            && _selectedTarget is not null
            && !string.IsNullOrWhiteSpace(_targetForAdd_Edit.Name)
            && (_targetForAdd_Edit.Name != _selectedTarget.Name
            || _targetForAdd_Edit.Note != _selectedTarget.Note)
            ;

        ///<summary>Логика выполнения - изменить цель</summary>
        private async Task OnEditTargetCommandExecuted(object? p)
        {
            _selectedTarget!.Name = _targetForAdd_Edit.Name;
            _selectedTarget.Note = _targetForAdd_Edit.Note;
            await _targetRepository.UpdateAsync(await _targetRepository.GetAsync(_selectedTarget.Id));
            EditTarget = false;
            SelectedTargetsView.Refresh();
        }

        #endregion

        #region RemoveTargetCommand - Команда - удалить цель

        ///<summary>Команда - удалить цель</summary>
        private ICommand? _removeTargetCommand;

        ///<summary>Команда - удалить цель</summary>
        public ICommand RemoveTargetCommand => _removeTargetCommand
            ??= new LambdaCommandAsync(OnRemoveTargetCommandExecuted, CanRemoveTargetCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить цель</summary>
        private bool CanRemoveTargetCommandExecute(object? p) => !ShowAdd_EditUserControl
            && !ShowAdd_EditTargetUserControl
            && _selectedTarget is not null
            ;

        ///<summary>Логика выполнения - удалить цель</summary>
        private async Task OnRemoveTargetCommandExecuted(object? p)
        {
            bool isComplete = _selectedTarget.IsComplete;
            await _targetRepository.RemoveAsync(_selectedTarget.Id);
            Targets.Remove(_selectedTarget);
            if (isComplete) CompletedTargetsCount--;
            else DepedencyProperites(this, nameof(CompletedTargetsCount));
            OnPropertyChanged(nameof(TargetsCount));
        }

        #endregion

        #region FilterCommand - Команда - фильтровать список

        ///<summary>Команда - фильтровать список</summary>
        private ICommand? _filterCommand;

        ///<summary>Команда - фильтровать список</summary>
        public ICommand FilterCommand => _filterCommand
            ??= new LambdaCommand<string>(OnFilterCommandExecuted, CanFilterCommandExecute);

        ///<summary>Проверка возможности выполнения - фильтровать список</summary>
        private bool CanFilterCommandExecute(string p) => TargetsCount > 10;

        ///<summary>Логика выполнения - фильтровать список</summary>
        private void OnFilterCommandExecuted(string p)
        {
            _filter = p.ToLower();
            ClearTargets();
            switch (_filter)
            {
                case "все":
                    foreach (var target in _selectedTargetsGroup.Targets)
                        Targets.Add(new TargetModel(target));
                    break;
                case "выполненные":
                case "невыполненные":
                    var isComplete = !_filter.StartsWith("не");
                    foreach (var target in _selectedTargetsGroup.Targets.Where(t => t.IsComplete == isComplete))
                        Targets.Add(new TargetModel(target));
                    break;
                default:
                    break;
            }
        }

        #endregion

        #endregion

        #region Methods...

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                CleanUpBehaviors = true;
                Targets.CollectionChanged -= Targets_CollectionChanged;
                ClearTargets();

                if (disposing)
                {
                    GroupsTargets.Clear();
                }
                _disposed = true;
            }
        }

        #endregion

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

        protected override void MethodBeforeAddElement()
        {
            if (AddElement)
            {
                TargetsGroupForAdd_Edit.Year = GroupsTargets.Last().Year == 0 ? (uint)DateTime.Now.Year : GroupsTargets.Last().Year + 1;
                TargetsGroupForAdd_Edit.Name = string.Empty;
                OnPropertyChanged(nameof(TargetsGroupForAdd_Edit));
            }
            DepedencyProperites(this, nameof(EnableFrameworkElements));
        }

        protected override void MethodBeforeEditElement()
        {
            if (EditElement)
            {
                TargetsGroupForAdd_Edit.Year = _selectedTargetsGroup.Year;
                TargetsGroupForAdd_Edit.Name = _selectedTargetsGroup.Name;
                OnPropertyChanged(nameof(TargetsGroupForAdd_Edit));
            }
            DepedencyProperites(this, nameof(EnableFrameworkElements));
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
