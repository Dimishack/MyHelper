using MyHelper.Infrastructure.Commands;
using MyHelper.Models.Challenges;
using MyHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ListChallengesUCViewModel : ViewModel
    {
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
        public MyChallenges? SelectedGroup { get => _selectedGroup; set => Set(ref _selectedGroup, value); }

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

        private void ListChallenges_ListChanged(object? sender, ListChangedEventArgs e)
        {
            var challengeOnProgressing = SelectedGroup?.ListChallenges.FirstOrDefault(
                c => c.IsProgress
                && string.IsNullOrWhiteSpace(c.DateStartProgressing));
            if (challengeOnProgressing is not null)
            {
                SelectedGroup.ListChallenges[challengeOnProgressing.Id].DateStartProgressing = DateTime.Now.ToString("yyyy.MM.dd");
                OnPropertyChanged(nameof(ChallengesOnProgress));
                return;
            }
            var challengesFromProgressing = SelectedGroup?.ListChallenges.FirstOrDefault(
                c => !c.IsProgress
                && !string.IsNullOrWhiteSpace(c.DateStartProgressing));
            if ( challengesFromProgressing is not null )
            {
                SelectedGroup.ListChallenges[challengesFromProgressing.Id].DateStartProgressing = string.Empty;
                OnPropertyChanged(nameof(ChallengesOnProgress));
            }
        }

        #endregion

        #endregion

    }
}
