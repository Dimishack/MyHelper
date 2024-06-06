using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Models.Challenges;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ListChallengesUCViewModel(IOpenWindows openWindows, IUserDialog userDialog, IWorkWithJSONFile workWithJSONFile) : ViewModel
    {
        private static readonly DateTime DATENOW = DateTime.Today;
        private readonly IOpenWindows _openWindows = openWindows;
        private readonly IUserDialog _userDialog = userDialog;
        private readonly IWorkWithJSONFile _workWithJSONFile = workWithJSONFile;

        private Dictionary<string, int> _forLengthChecklist = new()
        {
            {"Месяц", DateTime.DaysInMonth(DATENOW.Year, DATENOW.Month) },
            {"Квартал", DateTime.DaysInMonth(DATENOW.Year, DATENOW.Month)
                + DateTime.DaysInMonth(DATENOW.Year, DATENOW.Month + 1)
                + DateTime.DaysInMonth(DATENOW.Year, DATENOW.Month + 2)},
            {"Полгода", (DateTime.IsLeapYear(DATENOW.Year)? 366: 365) / 2 },
            {"Год", DateTime.IsLeapYear(DATENOW.Year)? 366: 365}
        };

        public string[] Groups { get; } = ["Все", "Месяц", "Квартал", "Полгода", "Год"];

        #region SelectedGroup : string - Выбранная группа

        ///<summary>Выбранная группа</summary>
        private string _selectedGroup = string.Empty;

        ///<summary>Выбранная группа</summary>
        public string SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (!Set(ref _selectedGroup, value)) return;

                if (value.Contains("все", StringComparison.OrdinalIgnoreCase))
                    ChallengesView = _challenges;
                else
                    ChallengesView = new(_challenges.Where(c => c.Duration == value).ToList());
                OnPropertyChanged(nameof(ChallengesView));
                OnPropertyChanged(nameof(ChallengesOnProgress));
            }
        }

        #endregion

        #region Challenges : ObservableCollection<MyChallenges> - Список челленджей

        ///<summary>Список челленджей</summary>
        private BindingList<MyChallenge>? _challenges;

        ///<summary>Список челленджей</summary>
        public BindingList<MyChallenge>? Challenges { get => _challenges; set => Set(ref _challenges, value); }

        #endregion

        #region ChallengesView : IList<MyChallenge> - Вывод списка челленджей

        ///<summary>Вывод списка челленджей</summary>
        private BindingList<MyChallenge>? _challengesView;

        ///<summary>Вывод списка челленджей</summary>
        public BindingList<MyChallenge>? ChallengesView { get => _challengesView; set => Set(ref _challengesView, value); }

        #endregion

        /// <summary> Челленджи в прогрессе</summary>
        public IList<MyChallenge>? ChallengesOnProgress =>
            (_challengesView)?.Where(c => c.IsProgress).ToList();

        #region SelectedChallenge : MyChallenge? - Выбранный челлендж

        ///<summary>Выбранный челлендж</summary>
        private MyChallenge? _selectedChallenge;

        ///<summary>Выбранный челлендж</summary>
        public MyChallenge? SelectedChallenge { get => _selectedChallenge; set => Set(ref _selectedChallenge, value); }

        #endregion

        #region Команды

        #region LoadedCommand - Команда - Загрузка

        ///<summary>Команда - Загрузка</summary>
        private ICommand? _loadedCommand;

        ///<summary>Команда - Загрузка</summary>
        public ICommand LoadedCommand => _loadedCommand
            ??= new LambdaCommand(OnLoadedCommandExecuted, CanLoadedCommandExecute);

        ///<summary>Проверка возможности выполнения - Загрузка</summary>
        private bool CanLoadedCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - Загрузка</summary>
        private void OnLoadedCommandExecuted(object? p)
        {
            if (_challenges is not null) return;

            if (_workWithJSONFile.ReadFile(@"Data\Challenges.json", out IList<MyChallenge>? challenges)
                && challenges is not null)
            {
                Challenges = new(challenges);
                ((Command)SaveChallengesCommand).Executable = false;
            }
            else
                Challenges = [];
            Challenges.ListChanged += ListChallenges_ListChanged;
            SelectedGroup = "Все";
        }

        #endregion

        #region OpenCheckListCommand - Команда - открыть чек-лист

        ///<summary>Команда - открыть чек-лист</summary>
        private ICommand? _openChecklistCommand;

        ///<summary>Команда - открыть чек-лист</summary>
        public ICommand OpenCheckListCommand => _openChecklistCommand
            ??= new LambdaCommand(OnOpenCheckListCommandExecuted, CanOpenCheckListCommandExecute);

        ///<summary>Проверка возможности выполнения - открыть чек-лист</summary>
        private bool CanOpenCheckListCommandExecute(object? p) => p is MyChallenge;

        ///<summary>Логика выполнения - открыть чек-лист</summary>
        private void OnOpenCheckListCommandExecuted(object? p)
        {
            _openWindows.OpenChecklistChallengeWindow((p as MyChallenge)!);
            ((Command)SaveChallengesCommand).Executable = true;
        }

        #endregion

        #region CreateChallengeCommand - Команда - создать челлендж

        ///<summary>Команда - создать челлендж</summary>
        private ICommand? _createChallengeCommand;

        ///<summary>Команда - создать челлендж</summary>
        public ICommand CreateChallengeCommand => _createChallengeCommand
            ??= new LambdaCommand(OnCreateChallengeCommandExecuted);

        ///<summary>Логика выполнения - создать челлендж</summary>
        private void OnCreateChallengeCommandExecuted(object? p)
        {
            var challenge = new MyChallenge();
            if (!_openWindows.OpenCreator_EditorChallengeWindow(challenge, _selectedGroup, "Создать челлендх")) return;

            challenge.Id = _challenges.Count;
            Challenges.Add(challenge);
            if (!ChallengesView.Contains(challenge))
                ChallengesView.Add(challenge);
            _userDialog.InformationMessage("Челлендж добавлен!");
        }

        #endregion

        #region EditChallengeCommand - Команда - редактировать челлендж

        ///<summary>Команда - редактировать челлендж</summary>
        private ICommand? _editChallengeCommand;

        ///<summary>Команда - редактировать челлендж</summary>
        public ICommand EditChallengeCommand => _editChallengeCommand
            ??= new LambdaCommand(OnEditChallengeCommandExecuted, CanEditChallengeCommandExecute);

        ///<summary>Проверка возможности выполнения - редактировать челлендж</summary>
        private bool CanEditChallengeCommandExecute(object? p) => _selectedChallenge is not null;

        ///<summary>Логика выполнения - редактировать челлендж</summary>
        private void OnEditChallengeCommandExecuted(object? p)
        {
            if (!_openWindows.OpenCreator_EditorChallengeWindow(
                _selectedChallenge!,
                _selectedChallenge!.Duration,
                "Редактировать челлендж")) return;
            CollectionViewSource.GetDefaultView(ChallengesView).Refresh();
            OnPropertyChanged(nameof(ChallengesOnProgress));
            ((Command)SaveChallengesCommand).Executable = true;
            _userDialog.InformationMessage("Челлендж отредактирован!");
        }

        #endregion

        #region DeleteChallengeCommand - Команда - удалить челлендж

        ///<summary>Команда - удалить челлендж</summary>
        private ICommand? _deleteChallengeCommand;

        ///<summary>Команда - удалить челлендж</summary>
        public ICommand DeleteChallengeCommand => _deleteChallengeCommand
            ??= new LambdaCommand(OnDeleteChallengeCommandExecuted, CanDeleteChallengeCommandExecute);

        ///<summary>Проверка возможности выполнения - удалить челлендж</summary>
        private bool CanDeleteChallengeCommandExecute(object? p) => p is MyChallenge;

        ///<summary>Логика выполнения - удалить челлендж</summary>
        private void OnDeleteChallengeCommandExecuted(object? p)
        {
            var challenge = (p as MyChallenge)!;
            Challenges.Remove(challenge);
            ChallengesView.Remove(challenge);
        }

        #endregion

        #region SaveChallengesCommand - Команда - сохранить челленджи

        ///<summary>Команда - сохранить челленджи</summary>
        private ICommand? _saveChallengesCommand;

        ///<summary>Команда - сохранить челленджи</summary>
        public ICommand SaveChallengesCommand => _saveChallengesCommand
            ??= new LambdaCommand(OnSaveChallengesCommandExecuted, CanSaveChallengesCommandExecute);

        ///<summary>Проверка возможности выполнения - сохранить челленджи</summary>
        private bool CanSaveChallengesCommandExecute(object? p) =>
            p is not null
            && p is IList<MyChallenge>;

        ///<summary>Логика выполнения - сохранить челленджи</summary>
        private void OnSaveChallengesCommandExecuted(object? p)
        {
            if (_workWithJSONFile.WriteFileAsync(@"Data\Challenges.json", p) == Task.FromResult(false)) return;
            _userDialog.InformationMessage("Список челленджей сохранен!");
            ((Command)SaveChallengesCommand).Executable = false;
        }

        #endregion

        #endregion

        #region События

        private void ListChallenges_ListChanged(object? sender, ListChangedEventArgs e)
        {
            try
            {
                var index = 0;
                if ((index = e.OldIndex) == -1) return;
                var isPropgress = _challenges[index].IsProgress;
                if (isPropgress)
                {
                    var startDate = _openWindows.OpenSelectStartDateWindow(_challenges[index]);
                    Challenges[index].DateStartProgressing = startDate;
                    Challenges[index].Checklist =
                    Enumerable.Range(0, _forLengthChecklist[_challenges[index].Duration!]).Select(i => new Checklist
                    {
                        NumberDay = i + 1,
                        Date = startDate.AddDays(i)
                    }).ToList();
                }
                else
                {
                    Challenges[index].DateStartProgressing = null;
                    Challenges[index].Checklist = null;
                }
            }
            catch (Exception) { }
            finally
            {
                OnPropertyChanged(nameof(ChallengesOnProgress));
                ((Command)SaveChallengesCommand).Executable = true;
            }
        }

        #endregion
    }
}
