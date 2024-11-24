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
    internal class ChallengesUCViewModel(IRepository<Challenge> challengeRepository, IRepository<Check> checkRepository) : ViewModel
    {
        private readonly IRepository<Challenge> _challengeRepository = challengeRepository;
        private readonly IRepository<Check> _checkRepository = checkRepository;

        #region Properties...

        #region Chellenges : ObservableCollection<ChallengeModel> - Список челленджей

        /// <summary>Список челленджей</summary>
        public ObservableCollection<ChallengeModel> Challenges { get; } = [];

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
                if(!Set(ref _selectedChallenge, value)) return;

                DepedenciesChanged(this);
            }
        }

        #endregion

        #region SelectedProgressingChallenge : ChallengeModel - Выбранный челлендж в прогрессе

        ///<summary>Выбранный челлендж в прогрессе</summary>
        private ChallengeModel? _selectedProgressingChallenge;

        ///<summary>Выбранный челлендж в прогрессе</summary>
        public ChallengeModel? SelectedProgressingChallenge { get => _selectedProgressingChallenge; set => Set(ref _selectedProgressingChallenge, value); }

        #endregion

        private readonly CollectionViewSource _challengesViewSource = new();
        public ICollectionView ChallengesView => _challengesViewSource.View;

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

        #region Durations : ObservableCollection<string> - Продолжительность

        ///<summary>Продолжительность</summary>
        public ObservableCollection<string> Durations { get; } = ["Все", "Месяц", "Квартал", "Полгода", "Год", "Пользовательский"];

        #endregion

        #region SelectedDuration : string - Выбранная продолжительность

        ///<summary>Выбранная продолжительность</summary>
        private string _selectedDuration = "Все";

        ///<summary>Выбранная продолжительность</summary>
        public string SelectedDuration
        {
            get => _selectedDuration;
            set
            {
                if (!Set(ref _selectedDuration, value)) return;
                Challenges.Clear();
                var challenges = _challengeRepository.Items;
                switch (value)
                {
                    case "Все":
                        foreach (var challenge in challenges)
                            Challenges.Add(new ChallengeModel(challenge));
                        break;
                    case "Месяц":
                    case "Квартал":
                    case "Полгода":
                    case "Год":
                    case "Пользовательский":
                        foreach (var challenge in challenges.Where(c => c.InProgress && c.Duration == value))
                            Challenges.Add(new ChallengeModel(challenge));
                        break;
                    default:
                        throw new NotImplementedException("Данная функция не реализована!");
                }
            }
        }

        #endregion

        #region AddChallenge : bool - Добваить челлендж

        ///<summary>Добваить челлендж</summary>
        private bool _addChallenge;

        [PropertyChangedWith(nameof(ShowAdd_EditUserControl))]
        [PropertyChangedWith(nameof(EnableToggleButtons))]
        [PropertyChangedWith(nameof(EnableToggleButtonsProgressAndEdit))]
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
                ChangedWithProperties(this);
            }
        }

        #endregion

        #region ChallengeForAdd_Edit : ChallengeModel - Челлендж для создания и редактирования

        ///<summary>Челлендж для создания и редактирования</summary>
        private Challenge _challengeForAdd_Edit = new();

        ///<summary>Челлендж для создания и редактирования</summary>
        public Challenge ChallengeForAdd_Edit { get => _challengeForAdd_Edit; set => Set(ref _challengeForAdd_Edit, value); }

        #endregion

        #region ShowAdd_EditUserControl : bool - Отобразить окно создания и редактирования челленджей

        ///<summary>Отобразить окно создания и редактирования челленджей</summary>
        public bool ShowAdd_EditUserControl => AddChallenge;

        #endregion

        #region EnableToggleButtons : bool - Включить переключатели

        ///<summary>Включить переключатели</summary>
        public bool EnableToggleButtons => !AddChallenge;

        #endregion

        #region EnableToggleButtonsProgressAndEdit : bool - Включить переключатели для выполнения и редактирования челленджей

        [DependencyOn(nameof(SelectedChallenge))]
        ///<summary>Включить переключатели для выполнения и редактирования челленджей</summary>
        public bool EnableToggleButtonsProgressAndEdit => EnableToggleButtons && _selectedChallenge is not null;

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
            foreach (var challenge in _challengeRepository.Items)
                Challenges.Add(new ChallengeModel(challenge));
            _challengesViewSource.Source = Challenges;
            OnPropertyChanged(nameof(ChallengesView));
        }

        #endregion

        #region AddChallengeCommand - Команда - добавить челлендж

        ///<summary>Команда - добавить челлендж</summary>
        private ICommand? _addChallengeCommand;

        ///<summary>Команда - добавить челлендж</summary>
        public ICommand AddChallengeCommand => _addChallengeCommand
            ??= new LambdaCommand(OnAddChallengeCommandExecuted, CanAddChallengeCommandExecute);

        ///<summary>Проверка возможности выполнения - добавить челлендж</summary>
        private bool CanAddChallengeCommandExecute(object? p) => ShowAdd_EditUserControl
            && !string.IsNullOrWhiteSpace(_challengeForAdd_Edit.Name);

        ///<summary>Логика выполнения - добавить челлендж</summary>
        private void OnAddChallengeCommandExecuted(object? p)
        {
            var newChallenge = new Challenge()
            {
                Name = _challengeForAdd_Edit.Name,
                Note = _challengeForAdd_Edit.Note,
                InProgress = false
            };
            _challengeRepository.Add(newChallenge);
            Challenges.Add(new ChallengeModel(newChallenge));
            AddChallenge = false;
        }

        #endregion

        #region CancelAdd_EditCommand - Команда - отмена создания (удаления) челленджа

        ///<summary>Команда - отмена создания (удаления) челленджа</summary>
        private ICommand? _cancelAdd_EditCommand;

        ///<summary>Команда - отмена создания (удаления) челленджа</summary>
        public ICommand CancelAdd_EditCommand => _cancelAdd_EditCommand
            ??= new LambdaCommand(OnCancelAdd_EditCommandExecuted, CanCancelAdd_EditCommandExecute);

        ///<summary>Проверка возможности выполнения - отмена создания (удаления) челленджа</summary>
        private bool CanCancelAdd_EditCommandExecute(object? p) => ShowAdd_EditUserControl;

        ///<summary>Логика выполнения - отмена создания (удаления) челленджа</summary>
        private void OnCancelAdd_EditCommandExecuted(object? p) => AddChallenge = false;

        #endregion

        #endregion


        public ChallengesUCViewModel() : this(null, null) { }
    }
}
