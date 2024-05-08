using MyHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


	}
}
