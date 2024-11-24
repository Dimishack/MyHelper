using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Interfaces;
using MyHelper.Models.Challenges;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
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

		private readonly CollectionViewSource _challengesViewSource = new CollectionViewSource();
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

		#region Groups : ObservableCollection<string> - Группы

		///<summary>Группы</summary>
		public ObservableCollection<string> Groups { get; } = ["Все", "Месяц", "Квартал", "Полгода", "Год", "Определенное время"];

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

		#endregion


		public ChallengesUCViewModel() : this(null,null) { }
    }
}
