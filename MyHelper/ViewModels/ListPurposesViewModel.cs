using MyHelper.Models.Purposes;
using MyHelper.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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


		public ListPurposesViewModel()
		{
			MyPurposes = new ObservableCollection<MyPurposes>(Enumerable.Range(0, 10).Select(p => new MyPurposes
			{
				Year = DateTime.Now.Year + p,
				Name = $"Name {p}",
				ListPurposes = new(Enumerable.Range(1, 10000).Select(p => new MyPurpose
				{
					Purpose = p.ToString(),
				}).ToList())
			}));
		}
	}
}
