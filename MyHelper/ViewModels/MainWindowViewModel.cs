using MyHelper.Infrastructure.Commands;
using MyHelper.Models.Books;
using MyHelper.Models.Challenges;
using MyHelper.Models.MyTasks;
using MyHelper.Models.Purposes;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class MainWindowViewModel : ViewModel
    {

        private readonly IOpenWindows _openWindows = null;

        const string pathTasks = @"Data\Tasks.json";
        const string pathPurposes = @"Data\Purposes.json";
        const string pathBooks = @"Data\Books.json";
        const string pathChallenges = @"Data\Challenges.json";

        #region Команды

        #region CreateNewListPurposesCommand

        public ICommand CreateNewListPurposesCommand { get; }

        private bool CanCreateNewListPurposesCommandExecute(object p) => true;
        private void OnCreateNewListPurposesCommandExecuted(object p)
        {
            var newListPurposes = new MyPurposes()
            {
                Name = (Int32.Parse(MyPurposes[^1].Name ?? DateTime.Now.Year.ToString()) + 1).ToString(),
                ListPurposes = []
            };
            newListPurposes.ListPurposes.ListChanged += ListPurposes_ListChanged;
            MyPurposes.Add(newListPurposes);
        }

        #endregion

        #region SaveCollectionCommand

        public ICommand SaveCollectionCommand { get; }

        private bool CanSaveCollectionCommandExecute(object p) => true;
        private void OnSaveCollectionCommandExecuted(object p)
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            if (p is ObservableCollection<MyPurposes>)
                File.WriteAllText(pathPurposes, JsonConvert.SerializeObject(p, Formatting.Indented));
            if (p is ObservableCollection<MyBooks>)
                File.WriteAllText(pathBooks, JsonConvert.SerializeObject(p, Formatting.Indented));
            if (p is ObservableCollection<MyTasks>)
                File.WriteAllText(pathTasks, JsonConvert.SerializeObject(p, Formatting.Indented));
            MessageBox.Show("Список успешно сохранен", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        #endregion

        #endregion

        #region Свойства

        #region MyChallenges

        #region CountChallenges : int - Количество челленджей выбранного списка
        /// <summary>Количество челленджей выбранного списка</summary>
        private int _countChallenges;
        /// <summary>Количество челленджей выбранного списка</summary>
        public int CountChallenges
        {
            get => _countChallenges;
            set => Set(ref _countChallenges, value);
        }
        #endregion

        #region ChallengesOnProgressing : IEnumerable - Список челленджей выбранного списка на выполнении
        /// <summary>Список челленджей на выполнении выбранного списка</summary>
        private IList<MyChallenge>? _challengesOnProgressing;
        /// <summary>Список челленджей на выполнении выбранного списка</summary>
        public IList<MyChallenge>? ChallengesOnProgressing
        {
            get => _challengesOnProgressing;
            set
            {
                Set(ref _challengesOnProgressing, value);
                CountChallengesOnProgressing = _challengesOnProgressing.Count;
            }
        }
        #endregion

        #region CountChallengesOnProgressing : int - Количество челленджей выбранного списка на выполнении
        /// <summary>Количество челленджей выбранного списка</summary>
        private int _сountChallengesOnProgressing;
        /// <summary>Количество челленджей выбранного списка</summary>
        public int CountChallengesOnProgressing
        {
            get => _сountChallengesOnProgressing;
            set => Set(ref _сountChallengesOnProgressing, value);
        }
        #endregion

        #region IndexSelectedChallenge : int - Индекс выбранного челленджа
        private int _indexSelectedChallenge;
        public int IndexSelectedChallenge
        {
            get => _indexSelectedChallenge;
            set => Set(ref _indexSelectedChallenge, value);
        }
        #endregion

        #region IndexSelectedChallengeOnProgressing : int - Индекс выбранного челленджа на выполнении
        private int _indexSelectedChallengeOnProgressing;
        public int IndexSelectedChallengeOnProgressing
        {
            get => _indexSelectedChallengeOnProgressing;
            set => Set(ref _indexSelectedChallengeOnProgressing, value);
        }
        #endregion

        #endregion

        #region MyPurposes
        public ObservableCollection<MyPurposes> MyPurposes { get; }

        #region CompletedPurposes : ushort - Количество выполненных целей
        /// <summary> Количество выполненных целей. </summary>
        private ushort _completedPurposes;
        /// <summary> Количество выполненных целей. </summary>
        public ushort CompletedPurposes
        {
            get => _completedPurposes;
            set => Set(ref _completedPurposes, value);
        }
        #endregion

        #region ListPurposesCount : int - Количество целей в списке
        private int _listPurposesCount;
        /// <summary> Количество целей в списке. </summary>
        public int ListPurposesCount
        {
            get => _listPurposesCount;
            set => Set(ref _listPurposesCount, value);
        }
        #endregion

        #region SelectedMyPurposes : MyPurposes - Выбранный год списка целей
        /// <summary>Выбранный год списка целей. </summary>
        private MyPurposes? _selectedMyPurposes;

        /// <summary>Выбранный год списка целей. </summary>
        public MyPurposes? SelectedMyPurposes
        {
            get => _selectedMyPurposes;
            set
            {
                Set(ref _selectedMyPurposes, value);
                if (_selectedMyPurposes?.ListPurposes is not null)
                {
                    ListPurposesCount = _selectedMyPurposes.ListPurposes.Count;
                    CompletedPurposes = (ushort)_selectedMyPurposes.ListPurposes.Where(i => i.IsCompleted).Count();
                    if (ListPurposesCount > 0)
                        Procent = String.Format("{0:0.##}%", CompletedPurposes / (float)ListPurposesCount * 100F);
                }
            }
        }
        #endregion


        private string? _procent;
        public string? Procent
        {
            get => _procent;
            set => Set(ref _procent, value);
        }
        #endregion

        public ObservableCollection<MyBooks> MyBooks { get; }
        public ObservableCollection<MyTasks> MyTasks { get; }

        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            CreateNewListPurposesCommand = new LambdaCommand(OnCreateNewListPurposesCommandExecuted, CanCreateNewListPurposesCommandExecute);
            SaveCollectionCommand = new LambdaCommand(OnSaveCollectionCommandExecuted, CanSaveCollectionCommandExecute);

            #endregion

            if (File.Exists(pathTasks) &&
                JsonConvert.DeserializeObject<ObservableCollection<MyTasks>>(File.ReadAllText(pathTasks)) is ObservableCollection<MyTasks> tasks)
                MyTasks = new ObservableCollection<MyTasks>(tasks);
            else MyTasks = [];
            if (File.Exists(pathPurposes) &&
                JsonConvert.DeserializeObject<ObservableCollection<MyPurposes>>(File.ReadAllText(pathPurposes)) is ObservableCollection<MyPurposes> purposes)
                MyPurposes = new ObservableCollection<MyPurposes>(purposes);
            else
            {
                MyPurposes =
            [
                new()
                {
                    Name = "Все",
                    ListPurposes = [],
                },
                new()
                {
                    Name = (DateTime.Now.Year + 1).ToString(),
                    ListPurposes = [],
                }
            ];
            }
            if (File.Exists(pathBooks) &&
                JsonConvert.DeserializeObject<ObservableCollection<MyBooks>>(File.ReadAllText(pathBooks)) is ObservableCollection<MyBooks> books)
                MyBooks = new ObservableCollection<MyBooks>(books);
            else MyBooks = [];

            for (int i = 0; i < MyPurposes.Count; i++)
                MyPurposes[i].ListPurposes.ListChanged += ListPurposes_ListChanged;
        }

        public MainWindowViewModel(IOpenWindows openWindows) : this()
        {
            _openWindows = openWindows;
        }

        private void ListPurposes_ListChanged(object? sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                case ListChangedType.ItemChanged:
                    if (sender is not BindingList<MyPurpose> listPurposes) return;
                    CompletedPurposes = (ushort)listPurposes.Where(i => i.IsCompleted).Count();
                    ListPurposesCount = listPurposes.Count;
                    if (ListPurposesCount > 0)
                        Procent = String.Format("{0:0.#}%", CompletedPurposes / (float)ListPurposesCount * 100F);
                    break;
            }
        }

        //private void ListChallenges_ListChanged(object? sender, ListChangedEventArgs e)
        //{
        //    switch (e.ListChangedType)
        //    {
        //        case ListChangedType.ItemChanged:
        //        case ListChangedType.ItemDeleted:
        //            if (sender is not BindingList<MyChallenge> listChallenges) return;
        //            if (IndexSelectedChallenge > -1)
        //            {
        //                if (listChallenges[IndexSelectedChallenge].IsProgress && listChallenges[IndexSelectedChallenge].DateStartProgressing is null)
        //                {
        //                    listChallenges[IndexSelectedChallenge].DateStartProgressing
        //                        = DateTime.Now.ToString("yyyy.MM.dd");

        //                    DateTime dateNow = DateTime.Now;
        //                    int year = dateNow.Year;
        //                    int month = dateNow.Month;
        //                    int dayOfMonth = DateTime.DaysInMonth(year, month);
        //                    int dayOfYear = DateTime.IsLeapYear(year) ? 366 : 365;
        //                    var typeChallenges = new Dictionary<string, int>
        //                    {
        //                        {"Месяц", dayOfMonth},
        //                        {"Квартал", dayOfMonth + DateTime.DaysInMonth(year, month+1) + DateTime.DaysInMonth(year, month+2)},
        //                        {"Полгода", dayOfYear / 2 },
        //                        {"Год", dayOfYear }
        //                    };
        //                    var checklist = new List<Checklist>();
        //                    foreach (var value in Enumerable.Range(0, typeChallenges[SelectedMyChallenges.ListName]))
        //                        checklist.Add(new Checklist($"{dateNow.AddDays(value):yyyy.MM.dd} ({value + 1})", false));
        //                    listChallenges[IndexSelectedChallenge].Checklist = new List<Checklist>(checklist);
        //                    listChallenges[IndexSelectedChallenge].DateStartProgressing
        //                        = DateTime.Now.ToString("yyyy.MM.dd");
        //                }
        //                else if (!listChallenges[IndexSelectedChallenge].IsProgress && listChallenges[IndexSelectedChallenge].DateStartProgressing is not null)
        //                {
        //                    listChallenges[IndexSelectedChallenge].DateStartProgressing = default;
        //                    listChallenges[IndexSelectedChallenge].Checklist = null;
        //                }
                            
        //            }
        //            var listChallengesOnProgressing = listChallenges.Where(i => i.IsProgress).ToList();
        //            if (!ChallengesOnProgressing.SequenceEqual(listChallengesOnProgressing))
        //                ChallengesOnProgressing = listChallengesOnProgressing;
        //            if (CountChallengesOnProgressing != listChallengesOnProgressing.Count)
        //                CountChallengesOnProgressing = listChallengesOnProgressing.Count;
        //            if (CountChallenges != listChallenges.Count)
        //                CountChallenges = listChallenges.Count;
        //            break;
        //    }
        //}
    }
}
