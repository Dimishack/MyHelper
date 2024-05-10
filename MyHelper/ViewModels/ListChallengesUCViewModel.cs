using MyHelper.Infrastructure.Commands;
using MyHelper.Models.Challenges;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ListChallengesUCViewModel(IOpenWindows openWindows) : ViewModel
    {
        private readonly IOpenWindows _openWindows = openWindows;
        private static readonly DateTime DATENOW = DateTime.Today;

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


            Challenges = new(Enumerable.Range(0, 10000).Select(c => new MyChallenge
            {
                Id = c,
                Challenge = $"Challenge {c}",
                Duration = Groups[Random.Shared.Next(1, Groups.Length)],
                Checklist = []
            }).ToList());
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
        private void OnOpenCheckListCommandExecuted(object? p) => _openWindows.OpenChecklistChallengeWindow((p as MyChallenge)!);

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

        #endregion

        #region События

        private void ListChallenges_ListChanged(object? sender, ListChangedEventArgs e)
        {
            var isPropgress = _challenges[e.NewIndex].IsProgress;
            Challenges[e.NewIndex].DateStartProgressing = isPropgress ? DATENOW : null;
            Challenges[e.NewIndex].Checklist = isPropgress ?
                Enumerable.Range(0, _forLengthChecklist[_challenges[e.NewIndex].Duration!]).Select(i => new Checklist
                {
                    NumberDay = i + 1,
                    Date = DATENOW.AddDays(i)
                }).ToList()
                : null;
            OnPropertyChanged(nameof(ChallengesOnProgress));
        }

        #endregion
    }
}
