using MyHelper.Infrastructure.Commands;
using MyHelper.Models.Books;
using MyHelper.Models.Challenges;
using MyHelper.Models.MyTasks;
using MyHelper.Models.Purposes;
using MyHelper.Services.Interfaces;
using MyHelper.ViewModels.Base;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace MyHelper.ViewModels
{
    class MainWindowViewModel : ViewModel
    {

        const string pathBooks = @"Data\Books.json";
        private readonly IWorkWithJSONFile _workJSON;

        #region Команды

        #region SaveCollectionCommand

        public ICommand SaveCollectionCommand { get; }

        private bool CanSaveCollectionCommandExecute(object p) => true;
        private void OnSaveCollectionCommandExecuted(object p)
        {
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            if (p is ObservableCollection<MyBooks>)
                _workJSON.WriteFile(pathBooks, p);
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
        public ObservableCollection<MyTask> MyTasks { get; }

        #endregion

        public MainWindowViewModel(IWorkWithJSONFile workJSON)
        {
            #region Команды

            SaveCollectionCommand = new LambdaCommand(OnSaveCollectionCommandExecuted, CanSaveCollectionCommandExecute);

            #endregion

            _workJSON = workJSON;

            ObservableCollection<MyBooks>? books;
            if((books = _workJSON.ReadFile<ObservableCollection<MyBooks>>(pathBooks)) is not null)
                MyBooks = new ObservableCollection<MyBooks>(books);
            else MyBooks = [];
        }
    }
}
