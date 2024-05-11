using MyHelper.Infrastructure.Commands;
using MyHelper.Models.Books;
using MyHelper.Models.Challenges;
using MyHelper.Models.MyTasks;
using MyHelper.Models.Purposes;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
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

        public ListPurposesUserControlViewModel ListPurposes { get; }

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

        public ObservableCollection<MyBooks> MyBooks { get; }
        public ObservableCollection<MyTasks> MyTasks { get; }

        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            SaveCollectionCommand = new LambdaCommand(OnSaveCollectionCommandExecuted, CanSaveCollectionCommandExecute);

            #endregion

            if (File.Exists(pathTasks) &&
                JsonConvert.DeserializeObject<ObservableCollection<MyTasks>>(File.ReadAllText(pathTasks)) is ObservableCollection<MyTasks> tasks)
                MyTasks = new ObservableCollection<MyTasks>(tasks);
            else MyTasks = [];
            if (File.Exists(pathBooks) &&
                JsonConvert.DeserializeObject<ObservableCollection<MyBooks>>(File.ReadAllText(pathBooks)) is ObservableCollection<MyBooks> books)
                MyBooks = new ObservableCollection<MyBooks>(books);
            else MyBooks = [];
        }

        public MainWindowViewModel(ListPurposesUserControlViewModel listPurposes, IOpenWindows openWindows) : this()
        {
            ListPurposes = listPurposes;
            _openWindows = openWindows;
        }

    }
}
