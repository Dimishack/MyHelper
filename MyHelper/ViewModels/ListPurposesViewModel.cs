using MyHelper.Infrastructure.Commands;
using MyHelper.Infrastructure.Commands.Base;
using MyHelper.Models.Purposes;
using MyHelper.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
	class ListPurposesViewModel : ViewModel
	{
		public ObservableCollection<MyPurposes>? MyPurposes { get; }

		#region SelectedListMyPurposes : MyPurposes - Выбранный список целей

		///<summary>Выбранный список целей</summary>
		private MyPurposes? _selectedListMyPurposes;

		///<summary>Выбранный список целей</summary>
		public MyPurposes? SelectedListMyPurposes { get => _selectedListMyPurposes; set => Set(ref _selectedListMyPurposes, value); }

		#endregion

		#region SelectedMyPurpose : MyPurpose - Выбранная цель

		///<summary>Выбранная цель</summary>
		private MyPurpose? _selectedMyPurpose;

		///<summary>Выбранная цель</summary>
		public MyPurpose? SelectedMyPurpose { get => _selectedMyPurpose; set => Set(ref _selectedMyPurpose, value); }

		#endregion

		#region Команды
		
		#region CreateNewPurposeCommand - Команда создания цели

		///<summary>Команда создания цели</summary>
		private ICommand? _сreateNewPurposeCommand;

		///<summary>Команда создания цели</summary>
		public ICommand CreateNewPurposeCommand => _сreateNewPurposeCommand
			??= new LambdaCommand(OnCreateNewPurposeCommandExecuted, CanCreateNewPurposeCommandExecute);

		///<summary>Проверка возможности выполнения - Команда создания цели</summary>
		private bool CanCreateNewPurposeCommandExecute(object? p) => true;

		///<summary>Логика выполнения - Команда создания цели</summary>
		private void OnCreateNewPurposeCommandExecuted(object? p)
		{
			((Command)SaveListMyPurposesCommand).Executable = true;
		}

		#endregion

		#region DeletePurposeCommand - Команда удаления цели

		///<summary>Команда удаления цели</summary>
		private ICommand? _deletePurposeCommand;

		///<summary>Команда удаления цели</summary>
		public ICommand DeletePurposeCommand => _deletePurposeCommand
			??= new LambdaCommand(OnDeletePurposeCommandExecuted, CanDeletePurposeCommandExecute);

		///<summary>Проверка возможности выполнения - Команда удаления цели</summary>
		private bool CanDeletePurposeCommandExecute(object? p) => SelectedMyPurpose is not null;

		///<summary>Логика выполнения - Команда удаления цели</summary>
		private void OnDeletePurposeCommandExecuted(object? p)
        {
            ((Command)SaveListMyPurposesCommand).Executable = true;
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
            ((Command)SaveListMyPurposesCommand).Executable = true;
        }

		#endregion

		#region SaveListMyPurposesCommand - Команда сохранения списка целей

		///<summary>Команда сохранения списка целей</summary>
		private ICommand? _saveListMyPurposesCommand;

		///<summary>Команда сохранения списка целей</summary>
		public ICommand SaveListMyPurposesCommand => _saveListMyPurposesCommand
			??= new LambdaCommand(OnSaveListMyPurposesCommandExecuted, CanSaveListMyPurposesCommandExecute);

		///<summary>Проверка возможности выполнения - Команда сохранения списка целей</summary>
		private bool CanSaveListMyPurposesCommandExecute(object? p) => true;

		///<summary>Логика выполнения - Команда сохранения списка целей</summary>
		private void OnSaveListMyPurposesCommandExecuted(object? p)
        {
            ((Command)SaveListMyPurposesCommand).Executable = false;
        }

		#endregion

		#endregion


		public ListPurposesViewModel()
		{
			((Command)SaveListMyPurposesCommand).Executable = false;
			MyPurposes = new ObservableCollection<MyPurposes>(Enumerable.Range(0, 10).Select(p => new MyPurposes
			{
				Year = DateTime.Now.Year + p,
				Name = $"Name {p}",
				ListPurposes = new ObservableCollection<MyPurpose>(Enumerable.Range(1, 10000).Select(p => new MyPurpose
				{
					Purpose = p.ToString(),
				}))
			}));
		}
	}
}
