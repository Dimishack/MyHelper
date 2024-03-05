using MyHelper.Infrastructure.Commands;
using MyHelper.Models.Books;
using MyHelper.Models.Challenges;
using MyHelper.Models.MyTasks;
using MyHelper.Models.Purposes;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        public ListPurposesViewModel ListPurposes { get; }

        #region Команды

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
            if (p is ObservableCollection<MyChallenges>)
                File.WriteAllText(pathChallenges, JsonConvert.SerializeObject(p, Formatting.Indented));
            MessageBox.Show("Список успешно сохранен", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        #endregion

        #region OpenChecklistChallengeWindowCommand - Открытие 2-го окна

        public ICommand OpenChecklistChallengeWindowCommand { get; }
        private bool CanOpenChecklistChallengeWindowCommandExecute(object p) => true;
        private void OnOpenChecklistChallengeWindowCommandExecuted(object p)
        {
            _openWindows.OpenChecklistChallengeWindow();
        }

        #endregion
        #endregion

        #region Свойства

        #region MyChallenges
        public ObservableCollection<MyChallenges> MyChallenges { get; }

        #region SelectedMyChallenges : MyChallenges - Выбранный тип списка челленджей
        private MyChallenges? _selectedMyChallenges;
        public MyChallenges? SelectedMyChallenges
        {
            get => _selectedMyChallenges;
            set
            {
                Set(ref _selectedMyChallenges, value);
                CountChallenges = _selectedMyChallenges.ListChallenges.Count;
                ChallengesOnProgressing = _selectedMyChallenges.ListChallenges.Where(i => i.IsProgress).ToList();
            }
        }
        #endregion

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

        public ObservableCollection<MyBooks> MyBooks { get; }
        public ObservableCollection<MyTasks> MyTasks { get; }

        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            SaveCollectionCommand = new LambdaCommand(OnSaveCollectionCommandExecuted, CanSaveCollectionCommandExecute);
            OpenChecklistChallengeWindowCommand = new LambdaCommand(OnOpenChecklistChallengeWindowCommandExecuted, CanOpenChecklistChallengeWindowCommandExecute);

            #endregion

            if (File.Exists(pathChallenges) &&
                JsonConvert.DeserializeObject<ObservableCollection<MyChallenges>>(File.ReadAllText(pathChallenges))
                is ObservableCollection<MyChallenges> challenges)
                MyChallenges = new ObservableCollection<MyChallenges>(challenges);
            else
            {
                string[] typeChallenges = ["Месяц", "Квартал", "Полгода", "Год"];
                MyChallenges = new ObservableCollection<MyChallenges>(Enumerable.Range(0, typeChallenges.Length).Select(i => new MyChallenges
                {
                    ListName = typeChallenges[i],
                    ListChallenges = [],
                }));
            }
            if (File.Exists(pathTasks) &&
                JsonConvert.DeserializeObject<ObservableCollection<MyTasks>>(File.ReadAllText(pathTasks)) is ObservableCollection<MyTasks> tasks)
                MyTasks = new ObservableCollection<MyTasks>(tasks);
            else MyTasks = [];
            if (File.Exists(pathBooks) &&
                JsonConvert.DeserializeObject<ObservableCollection<MyBooks>>(File.ReadAllText(pathBooks)) is ObservableCollection<MyBooks> books)
                MyBooks = new ObservableCollection<MyBooks>(books);
            else MyBooks = [];

            for (int i = 0; i < MyChallenges.Count; i++)
                MyChallenges[i].ListChallenges.ListChanged += ListChallenges_ListChanged;
        }

        public MainWindowViewModel(ListPurposesViewModel listPurposes, IOpenWindows openWindows) : this()
        {
            ListPurposes = listPurposes;
            _openWindows = openWindows;
        }

        private void ListChallenges_ListChanged(object? sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                    if (sender is not BindingList<MyChallenge> listChallenges) return;
                    if (IndexSelectedChallenge > -1)
                    {
                        if (listChallenges[IndexSelectedChallenge].IsProgress && listChallenges[IndexSelectedChallenge].DateStartProgressing is null)
                        {
                            listChallenges[IndexSelectedChallenge].DateStartProgressing
                                = DateTime.Now.ToString("yyyy.MM.dd");

                            DateTime dateNow = DateTime.Now;
                            int year = dateNow.Year;
                            int month = dateNow.Month;
                            int dayOfMonth = DateTime.DaysInMonth(year, month);
                            int dayOfYear = DateTime.IsLeapYear(year) ? 366 : 365;
                            var typeChallenges = new Dictionary<string, int>
                            {
                                {"Месяц", dayOfMonth},
                                {"Квартал", dayOfMonth + DateTime.DaysInMonth(year, month+1) + DateTime.DaysInMonth(year, month+2)},
                                {"Полгода", dayOfYear / 2 },
                                {"Год", dayOfYear }
                            };
                            var checklist = new List<Checklist>();
                            foreach (var value in Enumerable.Range(0, typeChallenges[SelectedMyChallenges.ListName]))
                                checklist.Add(new Checklist($"{dateNow.AddDays(value):yyyy.MM.dd} ({value + 1})", false));
                            listChallenges[IndexSelectedChallenge].Checklist = new List<Checklist>(checklist);
                            listChallenges[IndexSelectedChallenge].DateStartProgressing
                                = DateTime.Now.ToString("yyyy.MM.dd");
                        }
                        else if (!listChallenges[IndexSelectedChallenge].IsProgress && listChallenges[IndexSelectedChallenge].DateStartProgressing is not null)
                        {
                            listChallenges[IndexSelectedChallenge].DateStartProgressing = default;
                            listChallenges[IndexSelectedChallenge].Checklist = null;
                        }
                            
                    }
                    var listChallengesOnProgressing = listChallenges.Where(i => i.IsProgress).ToList();
                    if (!ChallengesOnProgressing.SequenceEqual(listChallengesOnProgressing))
                        ChallengesOnProgressing = listChallengesOnProgressing;
                    if (CountChallengesOnProgressing != listChallengesOnProgressing.Count)
                        CountChallengesOnProgressing = listChallengesOnProgressing.Count;
                    if (CountChallenges != listChallenges.Count)
                        CountChallenges = listChallenges.Count;
                    break;
            }
        }
    }
}
