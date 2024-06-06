using MyHelper.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.ViewModels
{
    internal class ListTasksUCViewModel : ViewModel
    {
		#region Test : string - Проверочная строка

		///<summary>Проверочная строка</summary>
		private string _test = "Проверка на подключение";

		///<summary>Проверочная строка</summary>
		public string Test { get => _test; set => Set(ref _test, value); }

		#endregion

	}
}
