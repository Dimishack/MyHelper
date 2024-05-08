using MyHelper.Infrastructure.Commands;
using MyHelper.Models.Challenges;
using MyHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ListChallengesUCViewModel : ViewModel
    {
		#region Title : string - Заголовок окна

		///<summary>Заголовок окна</summary>
		private string _title = "Проверка";

		///<summary>Заголовок окна</summary>
		public string Title { get => _title; set => Set(ref _title, value); }

        #endregion

        #region Challenges : BindingList<MyChallenges> - Список челленджей

        ///<summary>Список челленджей</summary>
        private ObservableCollection<MyChallenges>? _challenges;

		///<summary>Список челленджей</summary>
		public ObservableCollection<MyChallenges>? Challenges { get => _challenges; set => Set(ref _challenges, value); }

        #endregion

        #region SelectedGroup : BindingList<MyChallenges>? - Выбранная группа

        ///<summary>Выбранная группа</summary>
        private MyChallenges? _selectedGroup;

		///<summary>Выбранная группа</summary>
		public MyChallenges? SelectedGroup { get => _selectedGroup; set => Set(ref _selectedGroup, value); }

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

			var groups = new string[] {"Все", "Месяц", "Квартал" , "Полгода", "Год"};

			Challenges = new(Enumerable.Range(0, groups.Length).Select(c => new MyChallenges
			{
				Group = groups[c],
				ListChallenges = new(Enumerable.Range(0,1000).Select(c => new MyChallenge
				{
					Challenge = $"Challenge {c}",
					DateStartProgressing = DateTime.Now.ToShortDateString(),
					Checklist = []
				}).ToList()),
			}));
		}

		#endregion

		#endregion

	}
}
