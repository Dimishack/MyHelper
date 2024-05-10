using MyHelper.Infrastructure.Commands;
using MyHelper.Models.Challenges;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        #region Challenges : ObservableCollection<MyChallenges> - Список челленджей

        ///<summary>Список челленджей</summary>
        private ObservableCollection<MyChallenges>? _challenges;

        ///<summary>Список челленджей</summary>
        public ObservableCollection<MyChallenges>? Challenges { get => _challenges; set => Set(ref _challenges, value); }

        #endregion

        #region SelectedGroup : MyChallenges? - Выбранная группа

        ///<summary>Выбранная группа</summary>
        private MyChallenges? _selectedGroup;

        ///<summary>Выбранная группа</summary>
        public MyChallenges? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (!Set(ref _selectedGroup, value)) return;

                OnPropertyChanged(nameof(ChallengesOnProgress));
            }
        }

        #endregion

        public IList<MyChallenge>? ChallengesOnProgress => SelectedGroup?.ListChallenges.Where(c => c.IsProgress).ToList();

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

            var groups = new string[] { "Все", "Месяц", "Квартал", "Полгода", "Год" };

            Challenges = new(Enumerable.Range(0, groups.Length).Select(c => new MyChallenges
            {
                Group = groups[c],
                ListChallenges = new(Enumerable.Range(0, 1000).Select(c => new MyChallenge
                {
                    Id = c,
                    Challenge = $"Challenge {c}",
                    Checklist = []
                }).ToList()),
            }));
            for (int i = 0; i < Challenges.Count; i++)
                Challenges[i].ListChallenges.ListChanged += ListChallenges_ListChanged;
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

        #endregion

        #region События

        private void ListChallenges_ListChanged(object? sender, ListChangedEventArgs e)
        {
            var challengeOnProgressing = SelectedGroup?.ListChallenges.FirstOrDefault(
                c => c.IsProgress
                && c.DateStartProgressing == null);
            if (challengeOnProgressing is not null)
            {
                SelectedGroup!.ListChallenges[challengeOnProgressing.Id].DateStartProgressing = DATENOW;
                SelectedGroup.ListChallenges[challengeOnProgressing.Id].Checklist
                    = Enumerable.Range(0, _forLengthChecklist[SelectedGroup.Group!]).Select(i => new Checklist
                    {
                        NumberDay = i + 1,
                        Date = DATENOW.AddDays(i)
                    }).ToList();
                OnPropertyChanged(nameof(ChallengesOnProgress));
                return;
            }
            var challengesFromProgressing = SelectedGroup?.ListChallenges.FirstOrDefault(
                c => !c.IsProgress
                && c.DateStartProgressing != null);
            if (challengesFromProgressing is not null)
            {
                SelectedGroup!.ListChallenges[challengesFromProgressing.Id].DateStartProgressing = null;
                SelectedGroup!.ListChallenges[challengesFromProgressing.Id].Checklist = null;
                OnPropertyChanged(nameof(ChallengesOnProgress));
            }
        }

        #endregion
    }
}
