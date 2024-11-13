using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Attributes;
using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Interfaces;
using MyHelper.Models.Purposes;
using MyHelper.Models.Targets;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class TartetsUCViewModel(IOpenWindows openWindows,
                                  IUserDialog userDialog,
                                  IWorkWithJSONFile workWithJSONFile,
                                  IRepository<TargetsGroup> targets) : ViewModel
    {
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IWorkWithJSONFile _workWithJSONFile = workWithJSONFile;
        private readonly IRepository<TargetsGroup> _targetsRepository = targets;

        #region Properties...

        #region Targets : ObservableCollection<TargetsGroup> - Список групп с целями

        ///<summary>Список групп с целями</summary>
        public ObservableCollection<TargetsModel> Targets { get; } = [];

        #endregion

        #region SelectedTargets : TargetsModel - Выбранный список целей

        ///<summary>Выбранный список целей</summary>
        private TargetsModel _selectedTargets;

        ///<summary>Выбранный список целей</summary>
        public TargetsModel SelectedTargets { get => _selectedTargets; set => Set(ref _selectedTargets, value); }

        #endregion














        public Dictionary<string, SortDescription> Sorting { get; } = new()
        {
            { "Сначала старые записи", new SortDescription("Id", ListSortDirection.Ascending) },
            { "Сначала новые записи", new SortDescription("Id", ListSortDirection.Descending) },
            { "Сначала выполненные", new SortDescription("IsCompleted", ListSortDirection.Descending) },
            { "Сначала невыполненные", new SortDescription("IsCompleted", ListSortDirection.Ascending) },
            { "Цели (по возрастанию)", new SortDescription("Purpose", ListSortDirection.Ascending) },
            { "Цели (по убыванию)", new SortDescription("Purpose", ListSortDirection.Descending) },
        };

        #region SelectedSorting : string - Выбранная сортировка

        ///<summary>Выбранная сортировка</summary>
        private string _selectedSorting = "Сначала старые записи";

        ///<summary>Выбранная сортировка</summary>
        public string SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                Set(ref _selectedSorting, value);
                if (_selectedListPurposesView.View is not null)
                {
                    _selectedListPurposesView.View.SortDescriptions.Clear();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    _selectedListPurposesView.View.SortDescriptions.Add(Sorting[value]);
                }
            }
        }

        #endregion

        #region ListMyPurposes : ObservableCollection<MyPurposes> - Коллекция списков целей

        ///<summary>Коллекция списков целей</summary>
        private ObservableCollection<MyPurposes>? _listMyPurposes;

        ///<summary>Коллекция списков целей</summary>
        public ObservableCollection<MyPurposes>? ListMyPurposes { get => _listMyPurposes; set => Set(ref _listMyPurposes, value); }

        #endregion

        #region SelectedListMyPurposes : MyPurposes - Выбранный список целей

        ///<summary>Выбранный список целей</summary>
        private MyPurposes? _selectedListMyPurposes;

        ///<summary>Выбранный список целей</summary>
        [DependencyOn(nameof(MyPurposesCount))]
        [DependencyOn(nameof(CompletingMyPurposesCount))]
        [DependencyOn(nameof(Percent))]
        [DependencyOn(nameof(OffsetLimeGreenColor))]
        public MyPurposes? SelectedListMyPurposes
        {
            get => _selectedListMyPurposes;
            set
            {
                if (!Set(ref _selectedListMyPurposes, value)) return;

                _selectedListPurposesView.Source = value?.ListPurposes;
                OnPropertyChanged(nameof(SelectedListPurposesView));
                SelectedSorting = "Сначала старые записи";
                UpdatePropertyChanged();
            }
        }

        #endregion

        private readonly CollectionViewSource _selectedListPurposesView = new();
        public ICollectionView SelectedListPurposesView => _selectedListPurposesView.View;

        #region SelectedMyPurpose : MyPurpose - Выбранная цель

        ///<summary>Выбранная цель</summary>
        private MyPurpose? _selectedMyPurpose;

        ///<summary>Выбранная цель</summary>
        public MyPurpose? SelectedMyPurpose { get => _selectedMyPurpose; set => Set(ref _selectedMyPurpose, value); }

        #endregion

        #region MyPurposesCount : int - Количество целей в списке

        /// <summary>Количество целей в списке</summary>
        public int MyPurposesCount => SelectedListMyPurposes is null
            ? 0
            : SelectedListMyPurposes.ListPurposes.Count;

        #endregion

        #region CompletingMyPurposesCount : int - Количество выполненных целей в списке

        /// <summary>Количество выполненных целей в списке</summary>
        public int CompletingMyPurposesCount => SelectedListMyPurposes is null
            ? 0
            : SelectedListMyPurposes.ListPurposes.Where(p => p.IsCompleted).Count();

        #endregion

        #region Percent : double - Процент выполненных целей

        /// <summary>Процент выполненных целей</summary>
        public double Percent => (double)CompletingMyPurposesCount / (MyPurposesCount == 0 ? 1 : MyPurposesCount);

        #endregion

        #region OffsetLimeGreenColor : double - Местоположение лаймового цвета в ProgressBar'e

        /// <summary>Местоположение лаймового цвета в ProgressBar'e</summary>
        public double OffsetLimeGreenColor => 2.0 - Percent;

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
            foreach (TargetsGroup targets in _targetsRepository.Items)
                Targets.Add(new TargetsModel(targets));
            SelectedTargets = Targets[^1];
        }

        #endregion

        #region CreateNewYearCommand - Команда создания нового списка целей

        ///<summary>Команда создания нового списка целей</summary>
        private ICommand? _createNewYearCommand;

        ///<summary>Команда создания нового списка целей</summary>
        public ICommand CreateNewYearCommand => _createNewYearCommand
            ??= new LambdaCommand(OnCreateNewYearCommandExecuted);

        ///<summary>Логика выполнения - Команда создания нового списка целей</summary>
        private void OnCreateNewYearCommandExecuted(object? p)
        {
            MyPurposes listPurposes = new()
            {
                Year = ListMyPurposes![^1].Year == 0 ?
                DateTime.Now.Year
                : ListMyPurposes![^1].Year + 1,
            };
            if (!_openWindows.OpenCreator_EditorYearWindow(listPurposes, "Создать год")) return;
            ListMyPurposes?.Add(listPurposes);
            ListMyPurposes![^1].ListPurposes.ListChanged += ListPurposes_ListChanged;
            _userDialog.InformationMessage("Новый список целей успешно добавлен", "MyHepler");
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region DeleteYearCommand - Команда удаления списка целей

        ///<summary>Команда удаления списка целей</summary>
        private ICommand? _deleteYearCommand;

        ///<summary>Команда удаления списка целей</summary>
        public ICommand DeleteYearCommand => _deleteYearCommand
            ??= new LambdaCommand(OnDeleteYearCommandExecuted, CanDeleteYearCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда удаления списка целей</summary>
        private bool CanDeleteYearCommandExecute(object? p) => SelectedListMyPurposes is not null
            && !SelectedListMyPurposes.Name.Equals("пожизненные цели", StringComparison.CurrentCultureIgnoreCase);

        ///<summary>Логика выполнения - Команда удаления списка целей</summary>
        private void OnDeleteYearCommandExecuted(object? p)
        {
            ListMyPurposes?.Remove(SelectedListMyPurposes!);
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region EditYearCommand - Команда редактирования списка целей

        ///<summary>Команда редактирования списка целей</summary>
        private ICommand? _editYearCommand;

        ///<summary>Команда редактирования списка целей</summary>
        public ICommand EditYearCommand => _editYearCommand
            ??= new LambdaCommand(OnEditYearCommandExecuted, CanEditYearCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда редактирования списка целей</summary>
        private bool CanEditYearCommandExecute(object? p) => SelectedListMyPurposes is not null
            && !SelectedListMyPurposes.Name.Equals("пожизненные цели", StringComparison.CurrentCultureIgnoreCase);

        ///<summary>Логика выполнения - Команда редактирования списка целей</summary>
        private void OnEditYearCommandExecuted(object? p)
        {
            if (!_openWindows.OpenCreator_EditorYearWindow(SelectedListMyPurposes!, "Редактировать год")) return;

            CollectionViewSource.GetDefaultView(ListMyPurposes).Refresh();
            _userDialog.InformationMessage("Список целей успешно отредактирован");
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region CreateNewPurposeCommand - Команда создания цели

        ///<summary>Команда создания цели</summary>
        private ICommand? _сreateNewPurposeCommand;

        ///<summary>Команда создания цели</summary>
        public ICommand CreateNewPurposeCommand => _сreateNewPurposeCommand
            ??= new LambdaCommand(OnCreateNewPurposeCommandExecuted, CanCreateNewPurposeCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда создания цели</summary>
        private bool CanCreateNewPurposeCommandExecute(object? p) => SelectedListMyPurposes is not null;

        ///<summary>Логика выполнения - Команда создания цели</summary>
        private void OnCreateNewPurposeCommandExecuted(object? p)
        {
            var purpose = new MyPurpose();
            if (!_openWindows.OpenCreator_EditorPurposeWindow(purpose, "Создать цель")) return;
            purpose.Id = SelectedListMyPurposes.ListPurposes.Count;
            SelectedListMyPurposes?.ListPurposes.Add(purpose);
            _userDialog.InformationMessage("Цель успешно добавлена в список");
        }

        #endregion

        #region DeletePurposeCommand - Команда удаления цели

        ///<summary>Команда удаления цели</summary>
        private ICommand? _deletePurposeCommand;

        ///<summary>Команда удаления цели</summary>
        public ICommand DeletePurposeCommand => _deletePurposeCommand
            ??= new LambdaCommand<MyPurpose>(OnDeletePurposeCommandExecuted);


        ///<summary>Логика выполнения - Команда удаления цели</summary>
        private void OnDeletePurposeCommandExecuted(MyPurpose p)
        {
            int index = _selectedListMyPurposes!.ListPurposes.IndexOf(p);
            for (int i = index + 1; i < _selectedListMyPurposes.ListPurposes.Count; i++)
                _selectedListMyPurposes.ListPurposes[i].Id--;
            SelectedListMyPurposes!.ListPurposes.RemoveAt(index);

        }

        #endregion

        #region EditMyPurposeCommand - Редактирование цели

        ///<summary>Редактирование цели</summary>
        private ICommand? _editMyPurposeCommand;

        ///<summary>Редактирование цели</summary>
        public ICommand EditMyPurposeCommand => _editMyPurposeCommand
            ??= new LambdaCommand(OnEditMyPurposeCommandExecuted, CanEditMyPurposeCommandExecute);

        ///<summary>Проверка возможности выполнения - Редактирование цели</summary>
        private bool CanEditMyPurposeCommandExecute(object? p) => SelectedMyPurpose is not null;

        ///<summary>Логика выполнения - Редактирование цели</summary>
        private void OnEditMyPurposeCommandExecuted(object? p)
        {
            if (!_openWindows.OpenCreator_EditorPurposeWindow(SelectedMyPurpose!, "Редактировать цель")) return;

            _selectedListPurposesView.View.Refresh();
            _userDialog.InformationMessage("Цель отредактирована");
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

        #endregion

        #region SaveListMyPurposesCommand - Команда сохранения списка целей

        ///<summary>Команда сохранения списка целей</summary>
        private ICommand? _saveListMyPurposesCommand;

        ///<summary>Команда сохранения списка целей</summary>
        public ICommand SaveListMyPurposesCommand => _saveListMyPurposesCommand
            ??= new LambdaCommandAsync(OnSaveListMyPurposesCommandExecuted);

        ///<summary>Логика выполнения - Команда сохранения списка целей</summary>
        private async Task OnSaveListMyPurposesCommandExecuted(object? p)
        {
            SelectedSorting = "Сначала старые записи";
            if (!await _workWithJSONFile.WriteFileAsync(@"Data/Purposes.json", p)) return;

            _userDialog.InformationMessage("Список целей успешно сохранен");
            ((Command)SaveListMyPurposesCommand).Executable = false;
        }

        #endregion

        #endregion

        #region Events...

        private void ListPurposes_ListChanged(object? sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    UpdatePropertyChanged(nameof(SelectedListMyPurposes));
                    ((Command)SaveListMyPurposesCommand).Executable = true;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Methods...

        private void UpdatePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var property = this.GetType().GetProperty(propertyName!);
            if (property is null) return;

            foreach (var attr in property.GetCustomAttributes(false))
            {
                if (attr is DependencyOnAttribute dA)
                    OnPropertyChanged(dA.Name);
            }
        }

        #endregion

        public TartetsUCViewModel() : this(null, null, null, null)
        {

        }
    }
}
