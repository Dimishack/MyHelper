using MyHelper.DAL.Entyties;
using MyHelper.Infrastructure.Commands;
using MyHelper.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class TasksUCViewModel(IRepository<MyTask> tasksRepository) : ViewModel
    {
        private readonly IRepository<MyTask> _tasksRepository = tasksRepository;
        private readonly ObservableCollection<MyTask> _tasks = [];

		#region Properties...

		private readonly CollectionViewSource _tasksViewSource = new();
		public ICollectionView TasksView => _tasksViewSource.View;

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
			foreach (var task in _tasksRepository.Items)
				_tasks.Add(task);
			_tasksViewSource.Source = _tasks;
			OnPropertyChanged(nameof(TasksView));
		}

		#endregion

		#endregion
	}
}
