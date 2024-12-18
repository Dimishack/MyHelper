using Microsoft.EntityFrameworkCore;
using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Attributes;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.Models.Challenges;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ChallengesUCViewModel(IRepository<Challenge> challengeRepository, IRepository<Check> checkRepository) : ViewModel, IDisposable
    {
        private readonly IRepository<Challenge> _challengeRepository = challengeRepository;
        private readonly IRepository<Check> _checkRepository = checkRepository;
        private int _challengesCount = 0;
        private int _challengesOnProgressCount = 0;
        private bool _disposed = false;

        #region Properties...

        #region Challenges : ObservableCollection<ChallengeModel> - Список челленджей

        /// <summary>Список челленджей</summary>
        public ObservableCollection<ChallengeModel> Challenges { get; } = [];

        #endregion

        #region ChallengesOnProgress : ObservableCollection<ChallengeModel> - Список челленджей на выполнении

        ///<summary>Список челленджей на выполнении</summary>
        public ObservableCollection<ChallengeModel> ChallengesOnProgress { get; } = [];

        #endregion

        #region SelectedChallenge : ChallengeModel - Выбранный челлендж

        ///<summary>Выбранный челлендж</summary>
        private ChallengeModel? _selectedChallenge;

        ///<summary>Выбранный челлендж</summary>
        public ChallengeModel? SelectedChallenge
        {
            get => _selectedChallenge;
            set
            {
                if (!Set(ref _selectedChallenge, value)) return;

                PropertiesChanged(this);
            }
        }

        #endregion

        #region SelectedProgressingChallenge : ChallengeModel - Выбранный челлендж в прогрессе

        ///<summary>Выбранный челлендж в прогрессе</summary>
        private ChallengeModel? _selectedProgressingChallenge;

        ///<summary>Выбранный челлендж в прогрессе</summary>
        public ChallengeModel? SelectedProgressingChallenge
        {
            get => _selectedProgressingChallenge;
            set
            {
                if (_selectedProgressingChallenge == value) return;
                if (value is not null && value.CheckList.Count == 0)
                {
                    var checklist = _challengeRepository.Items.Include(cs => cs.CheckList).FirstOrDefault(c => c.Id == value.Id)?.CheckList;
                    if (checklist is not null)
                    {
                        int progressCount = 0;
                        foreach (var check in checklist)
                        {
                            if (check.Checked) progressCount++;
                            value.CheckList.Add(new CheckModel(check));
                        }
                        value.ProgressCount = progressCount;
                    }
                }
                Set(ref _selectedProgressingChallenge, value);
                PropertiesChanged(this);
            }
        }

        #endregion

        #region SelectedCheck : CheckModel - Выбранный чек

        ///<summary>Выбранный чек</summary>
        private CheckModel? _selectedCheck;

        ///<summary>Выбранный чек</summary>
        public CheckModel? SelectedCheck { get => _selectedCheck; set => Set(ref _selectedCheck, value); }

        #endregion

        private readonly CollectionViewSource _challengesViewSource = new();
        public ICollectionView ChallengesView => _challengesViewSource.View;

        private readonly CollectionViewSource _challengesOnProgressViewSource = new();
        public ICollectionView ChallengesOnProgressView => _challengesOnProgressViewSource.View;

        private readonly CollectionViewSource _checklistViewSource = new();
        public ICollectionView ChecklistView => _checklistViewSource.View;

        #region Sorts : Dictionary<string, SortDescription> - Список сортировки

        ///<summary>Список сортировки</summary>
        public Dictionary<string, SortDescription> Sorts { get; } = new()
        {
            {"В порядке возврастания", new SortDescription("Id", ListSortDirection.Ascending)},
            {"В порядке убывания", new SortDescription("Id", ListSortDirection.Descending)},
            {"По челленджам (Z -> Я)", new SortDescription("Name", ListSortDirection.Ascending)},
            {"По челленджам (Я -> Z)", new SortDescription("Name", ListSortDirection.Descending)},
        };

        #endregion

        #region SelectedSort : KeyValuePair<string, SortDescription> - Выбранная сортировка

        ///<summary>Выбранная сортировка</summary>
        private KeyValuePair<string, SortDescription> _selectedSort;

        ///<summary>Выбранная сортировка</summary>
        public KeyValuePair<string, SortDescription> SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (!Set(ref _selectedSort, value)) return;
                _challengesViewSource.SortDescriptions.Insert(0, Sorts[value.Key]);
            }
        }

        #endregion

        #region AddChallenge : bool - Добваить челлендж

        ///<summary>Добваить челлендж</summary>
        private bool _addChallenge;
        ///<summary>Добваить челлендж</summary>
        public bool AddChallenge
        {
            get => _addChallenge;
            set
            {
                if (!Set(ref _addChallenge, value)) return;
                if (value)
                {
                    ChallengeForAdd_Edit.Name = string.Empty;
                    ChallengeForAdd_Edit.Note = null;
                    OnPropertyChanged(nameof(ChallengeForAdd_Edit));
                }
                PropertiesChanged(this);
            }
        }

        #endregion

        #region EditChallenge : bool - Редактировать челлендж

        ///<summary>Редактировать челлендж</summary>
        private bool _editChallenge;

        ///<summary>Редактировать челлендж</summary>
        public bool EditChallenge
        {
            get => _editChallenge;
            set
            {
                if (!Set(ref _editChallenge, value)) return;
                if (value)
                {
                    ChallengeForAdd_Edit.Name = _selectedChallenge.Name;
                    ChallengeForAdd_Edit.Note = _selectedChallenge.Note;
                    OnPropertyChanged(nameof(ChallengeForAdd_Edit));
                }
                PropertiesChanged(this);
            }
        }

        #endregion

        #region StartChallenge : bool - Начать челлендж

        ///<summary>Начать челлендж</summary>
        private bool _startChallenge;

        ///<summary>Начать челлендж</summary>
        public bool StartChallenge
        {
            get => _startChallenge;
            set
            {
                if (!Set(ref _startChallenge, value)) return;
                if (value)
                    ChallengeForStart.ReturnToMainValues();
                PropertiesChanged(this);
            }
        }

        #endregion

        #region ShowChecklist : bool - Показать чек-лист

        ///<summary>Показать чек-лист</summary>
        private bool _showChecklist;

        ///<summary>Показать чек-лист</summary>
        public bool ShowChecklist
        {
            get => _showChecklist;
            set
            {
                if (!Set(ref _showChecklist, value)) return;
                if (value)
                {
                    _checklistViewSource.Source = _selectedProgressingChallenge?.CheckList;
                    OnPropertyChanged(nameof(ChecklistView));
                    _checklistViewSource.SortDescriptions[0] = new SortDescription("NumberDay", ListSortDirection.Ascending);
                    SelectedCheck = _selectedProgressingChallenge?.CheckList.FirstOrDefault(c => c.Date == DateOnly.FromDateTime(DateTime.Today));
                }
            }
        }

        #endregion

        #region ChallengeForAdd_Edit : Challenge_NameAndNote - Челлендж для создания и редактирования

        ///<summary>Челлендж для создания и редактирования</summary>
        private Challenge _challengeForAdd_Edit = new();

        ///<summary>Челлендж для создания и редактирования</summary>
        public Challenge ChallengeForAdd_Edit { get => _challengeForAdd_Edit; set => Set(ref _challengeForAdd_Edit, value); }

        #endregion

        #region ChallengeForStart : Challenge_Start - Челлендж для старта

        ///<summary>Челлендж для старта</summary>
        private ChallengeModelStart _challengeForStart = new();

        ///<summary>Челлендж для старта</summary>
        public ChallengeModelStart ChallengeForStart { get => _challengeForStart; set => Set(ref _challengeForStart, value); }

        #endregion

        #region ShowAdd_EditUserControl : bool - Отобразить окно создания и редактирования

        [DependencyOn([nameof(AddChallenge), nameof(EditChallenge)])]
        [ChangesWithProperties(nameof(EnableElements), true)]
        ///<summary>Отобразить окно создания и редактирования</summary>
        public bool ShowAdd_EditUserControl => _addChallenge || _editChallenge;

        #endregion

        #region EnableElements : bool - Включить переключатели

        [DependencyOn(nameof(StartChallenge))]
        [ChangesWithProperties(nameof(EnableToggleButtonsProgressAndEdit))]
        ///<summary>Включить переключатели</summary>
        public bool EnableElements => !ShowAdd_EditUserControl && !_startChallenge;

        #endregion

        #region EnableToggleButtonsProgressAndEdit : bool - Включить переключатели для выполнения и редактирования челленджей

        [DependencyOn(nameof(SelectedChallenge))]
        ///<summary>Включить переключатели для выполнения и редактирования челленджей</summary>
        public bool EnableToggleButtonsProgressAndEdit => EnableElements && _selectedChallenge is not null && !_selectedChallenge.InProgress;

        #endregion

        #region EnableToggleButtonChecklist : bool - Включить переключатель отображения чек-листа

        [DependencyOn(nameof(SelectedProgressingChallenge))]
        ///<summary>Включить переключатель отображения чек-листа</summary>
        public bool EnableToggleButtonChecklist => _selectedProgressingChallenge is not null
            && _selectedProgressingChallenge.CheckList.Count > 0;

        #endregion

        #region CheckedProgressFilterAll : bool - Чек фильтра выполнения "Все"

        ///<summary>Чек фильтра выполнения "Все"</summary>
        private bool _checkedProgressFilterAll = true;

        ///<summary>Чек фильтра выполнения "Все"</summary>
        public bool CheckedProgressFilterAll { get => _checkedProgressFilterAll; set => Set(ref _checkedProgressFilterAll, value); }

        #endregion

        #region CheckedProcessFilterAll : bool - Чек фильтра процесса "Все"

        ///<summary>Чек фильтров процесса и продолжительности "Все"</summary>
        private bool _checkedProcessFilterAll = true;

        ///<summary>Чек фильтров процесса и продолжительности "Все"</summary>
        public bool CheckedProcessFilterAll { get => _checkedProcessFilterAll; set => Set(ref _checkedProcessFilterAll, value); }

        #endregion

        #region CheckedDurationFilterAll : bool - Чек фильтра продолжительности "Все"

        ///<summary>Чек фильтра продолжительности "Все"</summary>
        private bool _checkDurationFilterAll = true;

        ///<summary>Чек фильтра продолжительности "Все"</summary>
        public bool CheckedDurationFilterAll { get => _checkDurationFilterAll; set => Set(ref _checkDurationFilterAll, value); }

        #endregion

        #region CleanUpBehaviors : bool - Очистить поведение

        ///<summary>Очистить поведение</summary>
        private bool _cleanUpBehaviors = false;

        ///<summary>Очистить поведение</summary>
        public bool CleanUpBehaviors { get => _cleanUpBehaviors; set => Set(ref _cleanUpBehaviors, value); }

        #endregion

        #endregion

        #region Commands...

        #region LoadCommand - Команда - загрузка пользовательского окна

        ///<summary>Команда - загрузка пользовательского окна</summary>
        private ICommand? _loadCommand;

        ///<summary>Команда - загрузка пользовательского окна</summary>
        public ICommand LoadCommand => _loadCommand
            ??= new LambdaCommand(OnLoadCommandExecuted);

        ///<summary>Логика выполнения - загрузка пользовательского окна</summary>
        private void OnLoadCommandExecuted(object? p)
        {
            foreach (var challenge in _challengeRepository.Items)
            {
                var newChallenge = new ChallengeModel(challenge);
                Challenges.Add(newChallenge);
                if (challenge.InProgress)
                {
                    newChallenge.CheckedChanged += NewChallenge_CheckedChanged;
                    ChallengesOnProgress.Add(newChallenge);
                }
            }
            _challengesCount = Challenges.Count;
            _challengesOnProgressCount = ChallengesOnProgress.Count;
            _challengesViewSource.Source = Challenges;
            OnPropertyChanged(nameof(ChallengesView));

            _challengesOnProgressViewSource.Source = ChallengesOnProgress;
            OnPropertyChanged(nameof(ChallengesOnProgressView));
            _challengesOnProgressViewSource.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Descending));
            _challengesOnProgressViewSource.SortDescriptions.Add(new SortDescription("DateStart", ListSortDirection.Ascending));
            _challengesOnProgressViewSource.SortDescriptions.Add(new SortDescription("DaysLeft", ListSortDirection.Ascending));
            ChallengesOnProgressView.MoveCurrentToFirst();

            _checklistViewSource.SortDescriptions.Add(new SortDescription("NumberDay", ListSortDirection.Ascending));

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

        #region AddChallengeCommand - Команда - добавить челлендж

        ///<summary>Команда - добавить челлендж</summary>
        private ICommand? _addChallengeCommand;

        ///<summary>Команда - добавить челлендж</summary>
        public ICommand AddChallengeCommand => _addChallengeCommand
            ??= new LambdaCommandAsync(OnAddChallengeCommandExecuted, CanAddChallengeCommandExecute);

        ///<summary>Проверка возможности выполнения - добавить челлендж</summary>
        private bool CanAddChallengeCommandExecute(object? p) => ShowAdd_EditUserControl
            && !string.IsNullOrWhiteSpace(_challengeForAdd_Edit.Name);

        ///<summary>Логика выполнения - добавить челлендж</summary>
        private async Task OnAddChallengeCommandExecuted(object? p)
        {
            var newChallenge = new Challenge()
            {
                Name = _challengeForAdd_Edit.Name,
                Note = _challengeForAdd_Edit.Note,
                InProgress = false
            };
            await _challengeRepository.AddAsync(newChallenge);
            Challenges.Add(new ChallengeModel(newChallenge));
            _challengesCount++;
            AddChallenge = false;
        }

        #endregion

        #region EditChallengeCommand - Команда - редактировать челлендж

        ///<summary>Команда - редактировать челлендж</summary>
        private ICommand? _editChallengeCommand;

        ///<summary>Команда - редактировать челлендж</summary>
        public ICommand EditChallengeCommand => _editChallengeCommand
            ??= new LambdaCommandAsync(OnEditChallengeCommandExecuted, CanEditChallengeCommandExecute);

        ///<summary>Проверка возможности выполнения - редактировать челлендж</summary>
        private bool CanEditChallengeCommandExecute(object? p) => ShowAdd_EditUserControl
            && _selectedChallenge is not null
            && !string.IsNullOrWhiteSpace(_challengeForAdd_Edit.Name)
            && (_selectedChallenge.Name != _challengeForAdd_Edit.Name
            || _selectedChallenge.Note != _challengeForAdd_Edit.Note)
            ;

        ///<summary>Логика выполнения - редактировать челлендж</summary>
        private async Task OnEditChallengeCommandExecuted(object? p)
        {
            SelectedChallenge.Name = ChallengeForAdd_Edit.Name;
            SelectedChallenge.Note = ChallengeForAdd_Edit.Note;
            await _challengeRepository.UpdateAsync(await _challengeRepository.GetAsync(_selectedChallenge.Id));
            ChallengesView.Refresh();
            EditChallenge = false;
        }

        #endregion

        #region StartChallengeCommand - Команда - начать челлендж

        ///<summary>Команда - начать челлендж</summary>
        private ICommand? _startChallengeCommand;

        ///<summary>Команда - начать челлендж</summary>
        public ICommand StartChallengeCommand => _startChallengeCommand
            ??= new LambdaCommandAsync(OnStartChallengeCommandExecutedAsync, CanStartChallengeCommandExecute);

        ///<summary>Проверка возможности выполнения - начать челлендж</summary>
        private bool CanStartChallengeCommandExecute(object? p)
        {
            var result = _startChallenge
            && _selectedChallenge is not null;
            if (_challengeForStart.Regularity == "По дням недели")
            {
                var atLeastOnDayOfWeek = false;
                foreach (var dayOfweek in _challengeForStart.DaysOfWeek)
                {
                    if (dayOfweek)
                    {
                        atLeastOnDayOfWeek = true;
                        break;
                    }
                }
                result = atLeastOnDayOfWeek;
            }

            return result;
        }

        ///<summary>Логика выполнения - начать челлендж</summary>
        private async Task OnStartChallengeCommandExecutedAsync(object? p)
        {
            var checklist = SelectedChallenge.StartChallenge(_challengeForStart);
            _selectedChallenge.CheckedChanged += NewChallenge_CheckedChanged;
            var getChallenge = await _challengeRepository.GetAsync(_selectedChallenge.Id);
            getChallenge.CheckList = checklist;
            ChallengesOnProgress.Add(_selectedChallenge);
            await _challengeRepository.UpdateAsync(getChallenge);
            _challengesOnProgressCount++;
            StartChallenge = false;
        }

        #endregion

        #region StopChallengeCommand - Команда - остановить челлендж

        ///<summary>Команда - остановить челлендж</summary>
        private ICommand? _stopChallengeCommand;

        ///<summary>Команда - остановить челлендж</summary>
        public ICommand StopChallengeCommand => _stopChallengeCommand
            ??= new LambdaCommandAsync(OnStopChallengeCommandExecuted, CanStopChallengeCommandExecute);

        ///<summary>Проверка возможности выполнения - остановить челлендж</summary>
        private bool CanStopChallengeCommandExecute(object? p) => !_showChecklist
            && _selectedProgressingChallenge is not null
            && _selectedProgressingChallenge.InProgress;

        ///<summary>Логика выполнения - остановить челлендж</summary>
        private async Task OnStopChallengeCommandExecuted(object? p)
        {
            SelectedProgressingChallenge.StopChallenge();
            _selectedProgressingChallenge.CheckedChanged -= NewChallenge_CheckedChanged;
            var getChallenge = await _challengeRepository.GetAsync(_selectedProgressingChallenge.Id);
            getChallenge.CheckList.Clear();
            Challenges.First(c => c.Id == _selectedProgressingChallenge.Id).StopChallenge();
            await _challengeRepository.UpdateAsync(getChallenge);
            ChallengesOnProgress.Remove(_selectedProgressingChallenge);
            if(--_challengesOnProgressCount <= 10)
            {
                if (!CheckedDurationFilterAll)
                {
                    CheckedDurationFilterAll = true;
                    OnDurationFilterCommandExecuted("Все"); 
                }
                if (!CheckedProcessFilterAll)
                {
                    CheckedProcessFilterAll = true;
                }
            }
        }

        #endregion

        #region HideAdditiionalCustomControl - Команда - скрыть дополнительное окно

        ///<summary>Команда - скрыть дополнительное окно</summary>
        private ICommand? _hideAdditionalCustomControl;

        ///<summary>Команда - скрыть дополнительное окно</summary>
        public ICommand HideAdditiionalCustomControl => _hideAdditionalCustomControl
            ??= new LambdaCommand(OnHideAdditiionalCustomControlExecuted, CanHideAdditiionalCustomControlExecute);

        ///<summary>Проверка возможности выполнения - скрыть дополнительное окно</summary>
        private bool CanHideAdditiionalCustomControlExecute(object? p) => ShowAdd_EditUserControl || StartChallenge;

        ///<summary>Логика выполнения - скрыть дополнительное окно</summary>
        private void OnHideAdditiionalCustomControlExecuted(object? p) => StartChallenge = AddChallenge = EditChallenge = false;

        #endregion

        #region DeleteChallengeCommand - Команда - удалить челлендж

        ///<summary>Команда - удалить челлендж</summary>
        private ICommand? _deleteChallengeCommand;

        ///<summary>Команда - удалить челлендж</summary>
        public ICommand DeleteChallengeCommand => _deleteChallengeCommand
            ??= new LambdaCommand(OnDeleteChallengeCommandExecuted, CanDeleteChallengeCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить челлендж</summary>
        private bool CanDeleteChallengeCommandExecute(object? p) => !ShowAdd_EditUserControl
            && _selectedChallenge is not null
            && !_selectedChallenge.InProgress
            ;

        ///<summary>Логика выполнения - удалить челлендж</summary>
        private void OnDeleteChallengeCommandExecuted(object? p)
        {
            _challengeRepository.Remove(_selectedChallenge.Id);
            Challenges.Remove(_selectedChallenge);
            if (--_challengesCount <= 10 && !_checkedProgressFilterAll)
            {
                CheckedProgressFilterAll = true;
                OnProgressFilterCommandExecuted("Все");
            }
        }

        #endregion

        #region SaveRepositoryCommand - Команда - сохранить репозиторий челленджей

        ///<summary>Команда - сохранить репозиторий челленджей</summary>
        private ICommand? _saveRepositoryCommand;

        ///<summary>Команда - сохранить репозиторий челленджей</summary>
        public ICommand SaveRepositoryCommand => _saveRepositoryCommand
            ??= new LambdaCommandAsync(OnSaveRepositoryCommandExecuted, CanSaveRepositoryCommandExecute);

        ///<summary>Проверка возможности выполнения - сохранить репозиторий челленджей</summary>
        private bool CanSaveRepositoryCommandExecute(object? p) => _challengeRepository is not null && !_challengeRepository.AutoSaveChanges;

        ///<summary>Логика выполнения - сохранить репозиторий челленджей</summary>
        private async Task OnSaveRepositoryCommandExecuted(object? p) => await _challengeRepository.SaveChangedAsync();

        #endregion

        #region ProgressFilterCommand - Команда - фильтровать выполнение (Челленджи)

        ///<summary>Команда - фильтровать выполнение (Челленджи)</summary>
        private ICommand? _progressFilterCommand;

        ///<summary>Команда - фильтровать выполнение (Челленджи)</summary>
        public ICommand ProgressFilterCommand => _progressFilterCommand
            ??= new LambdaCommand<string>(OnProgressFilterCommandExecuted, CanProgressFilterCommandExecute);

        ///<summary>Проверка возможности выполнения - фильтровать выполнение (Челленджи)</summary>
        private bool CanProgressFilterCommandExecute(string p) => _challengesCount > 10;

        ///<summary>Логика выполнения - фильтровать выполнение (Челленджи)</summary>
        private void OnProgressFilterCommandExecuted(string p)
        {
            var challenges = _challengeRepository.Items;
            Challenges.Clear();

            switch (p)
            {
                case "Все":
                    foreach (var challenge in challenges)
                        Challenges.Add(new ChallengeModel(challenge));
                    break;
                case "Не выполняются":
                    foreach (var challenge in challenges.Where(c => !c.InProgress))
                        Challenges.Add(new ChallengeModel(challenge));
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region DurationFilterCommand - Команда - фильтровать список по продолжительности (Челленджи на выполнении)

        ///<summary>Команда - фильтровать список по продолжительности (Челленджи на выполнении)</summary>
        private ICommand? _durationFilterCommand;

        ///<summary>Команда - фильтровать список по продолжительности (Челленджи на выполнении)</summary>
        public ICommand DurationFilterCommand => _durationFilterCommand
            ??= new LambdaCommand<string>(OnDurationFilterCommandExecuted, CanDurationFilterCommandExecute);

        ///<summary>Проверка возможности выполнения - фильтровать список по продолжительности (Челленджи на выполнении)</summary>
        private bool CanDurationFilterCommandExecute(string p) => _challengesOnProgressCount > 10;

        ///<summary>Логика выполнения - фильтровать список по продолжительности (Челленджи на выполнении)</summary>
        private void OnDurationFilterCommandExecuted(string p)
        {

            var challenges = _challengeRepository.Items.Where(c => c.InProgress);
            ChallengesOnProgress.Clear();
            switch (p)
            {
                case "Все":
                    foreach (var challenge in challenges)
                        ChallengesOnProgress.Add(new ChallengeModel(challenge));
                    break;
                case "Месяц":
                    foreach (var challenge in challenges.Where(c => c.Duration == "Месяц"))
                        ChallengesOnProgress.Add(new ChallengeModel(challenge));
                    break;
                case "Квартал":
                    foreach (var challenge in challenges.Where(c => c.Duration == "Квартал"))
                        ChallengesOnProgress.Add(new ChallengeModel(challenge));
                    break;
                case "Полгода":
                    foreach (var challenge in challenges.Where(c => c.Duration == "Полгода"))
                        ChallengesOnProgress.Add(new ChallengeModel(challenge));
                    break;
                case "Беременность":
                    foreach (var challenge in challenges.Where(c => c.Duration == "Беременность"))
                        ChallengesOnProgress.Add(new ChallengeModel(challenge));
                    break;
                case "Год":
                    foreach (var challenge in challenges.Where(c => c.Duration == "Год"))
                        ChallengesOnProgress.Add(new ChallengeModel(challenge));
                    break;
                default:
                    break;
            }
        }

        #endregion

        #endregion

        private async void NewChallenge_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is CheckModel check)
                await _checkRepository.UpdateAsync(await _checkRepository.GetAsync(check.Id));
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
                if (disposing) { }
                CleanUpBehaviors = true;
                foreach (var challengeOnProgress in ChallengesOnProgress)
                    challengeOnProgress.Dispose();
                ChallengesOnProgress.Clear();
                foreach (var challenge in Challenges)
                    challenge.Dispose();
                Challenges.Clear();
                _disposed = true;
            }
        }

        public ChallengesUCViewModel() : this(null, null) { }
    }
}
