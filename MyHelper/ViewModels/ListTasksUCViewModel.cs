using MyHelper.Infrastructure.Commands;
using MyHelper.Models.MyTasks;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    internal class ListTasksUCViewModel(IWorkWithJSONFile workWithJSONFile) : ViewModel
    {
        private readonly IWorkWithJSONFile _workWithJSONFile = workWithJSONFile;

        #region Свойства

        public Dictionary<string, SortDescription> Sorting { get; } = new()
        {
            {"Сначала старые записи", new("Id", ListSortDirection.Ascending) },
            {"Сначала новые записи", new("Id", ListSortDirection.Descending) },
            {"Задачи (по возрастанию)", new("Task", ListSortDirection.Ascending) },
            {"Задачи (по убыванию)", new("Task", ListSortDirection.Descending) },
            {"Сначала важные", new("Prompt", ListSortDirection.Descending) },
            {"Сначала срочные", new("Important", ListSortDirection.Descending) },
            {"Срок (по возрастанию)", new("Term", ListSortDirection.Ascending) },
            {"Срок (по убыванию)", new("Term", ListSortDirection.Descending) },
        };

        #region SelectedSorting : string - Выбранная сортировка

        ///<summary>Выбранная сортировка</summary>
        private string _selectedSorting = string.Empty;

        ///<summary>Выбранная сортировка</summary>
        public string SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                if (!Set(ref _selectedSorting, value)) return;

                _listTasksView.SortDescriptions.Clear();
                _listTasksView.SortDescriptions.Add(Sorting[value]);
            }
        }

        #endregion

        #region Groups : IList<string> - Группы

        ///<summary>Группы</summary>
        private IList<string> _groups = ["Все"];

        ///<summary>Группы</summary>
        public IList<string> Groups { get => _groups; set => Set(ref _groups, value); }

        #endregion

        private readonly CollectionViewSource _listTasksView = new();
        public ICollectionView ListTasksView => _listTasksView.View;

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

                if (value.Contains("Все"))
                    _listTasksView.Source = _listTasks?.ToList();
                else
                    _listTasksView.Source = _listTasks?.Where(i => i.Group == value).ToList();
                OnPropertyChanged(nameof(ListTasksView));
            }
        }

        #endregion

        #region ListTasks : ObservableCollection<MyTasks> - Список задач

        ///<summary>Список задач</summary>
        private ObservableCollection<MyTasks> _listTasks = [];

        ///<summary>Список задач</summary>
        public ObservableCollection<MyTasks> ListTasks { get => _listTasks; set => Set(ref _listTasks, value); }

        #endregion

        #endregion

        #region Команды

        #region LoadedCommand - Команда - загрузка окна

        ///<summary>Команда - загрузка окна</summary>
        private ICommand? _loadedCommand;

        ///<summary>Команда - загрузка окна</summary>
        public ICommand LoadedCommand => _loadedCommand
            ??= new LambdaCommand(OnLoadedCommandExecuted);

        ///<summary>Логика выполнения - загрузка окна</summary>
        private void OnLoadedCommandExecuted(object? p)
        {
            if (_listTasks.Count > 0) return;

            var groups = Enumerable.Range(0, 10).Select(i => $"Group {i}").ToList();
            if (_workWithJSONFile.ReadFile(@"Data/MyTasks.json", out IList<MyTasks>? listTasks) && listTasks is not null)
                ListTasks = new(listTasks);
            else
            {
                ListTasks = new(Enumerable.Range(0, 10000).Select(i => new MyTasks
                {
                    Id = i,
                    Task = $"Task {i}",
                    Group = groups[Random.Shared.Next(0, groups.Count)],
                }).ToList());
            }
            SelectedSorting = "Сначала старые записи";
            foreach (var group in _listTasks.Select(i => i.Group).Distinct().Order().ToList())
            {
                Groups.Add(group);
            }
            SelectedGroup = "Все";
        }

        #endregion

        #endregion

    }
}
