using MyHelper.Models.Challenges;
using MyHelper.ViewModels.Base;
using System.ComponentModel;

namespace MyHelper.ViewModels
{
    class ChecklistChallengeViewModel : ViewModel
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        #region Title : string? - Заголовок окна
        /// <summary>Заголовок окна</summary>
        private string? _title;
        /// <summary>Заголовок окна</summary>
        public string? Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        #endregion

        #region CheckLists : BindingList<Checklist>? - Чек-лист
        /// <summary>Чек-лист</summary>
        private BindingList<Checklist>? _checklists;
        /// <summary>Чек-лист</summary>
        public BindingList<Checklist>? Checklists
        {
            get => _checklists;
            set => Set(ref _checklists, value);
        }
        #endregion

        #region Count : int - Количество чека
        /// <summary>Количество чека</summary>
        private int _count;
        /// <summary>Количество чека</summary>
        public int Count
        {
            get => _count;
            set => Set(ref _count, value);
        }
        #endregion

        #region Progress : int - Количества выполненных чеков
        /// <summary>Количества выполненных чеков</summary>
        private int _progess;
        /// <summary>Количества выполненных чеков</summary>
        public int Progess
        {
            get => _progess;
            set => Set(ref _progess, value);
        }
        #endregion

        #region Procent : double - Процент выполненных чеков
        /// <summary>Процент выполненных чеков</summary>
        private double _procent;
        /// <summary>Процент выполненных чеков</summary>
        public double Procent
        {
            get => _procent;
            set => Set(ref _procent, value);
        } 
        #endregion

        public ChecklistChallengeViewModel() : this(null)
        {

        }

        public ChecklistChallengeViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            var list = _mainWindowViewModel?.ChallengesOnProgressing?[_mainWindowViewModel.IndexSelectedChallengeOnProgressing] ?? null;
            if (list is not null)
            {
                _title = list.Challenge;
                _checklists = new BindingList<Checklist>(list.Checklist!);
                _checklists.ListChanged += Checklists_ListChanged; 
                _count = _checklists?.Count ?? 0;
                _progess = _checklists?.Where(i => i.Check).Count() ?? 0;
                _procent = double.Round((double)_progess / (_count == 0 ? 1 : _count) * 100D, 2);
            }
        }

        private void Checklists_ListChanged(object? sender, ListChangedEventArgs e)
        {
            Count = _checklists?.Count ?? 0;
            Progess = _checklists?.Where(i => i.Check).Count() ?? 0;
            Procent = double.Round((double)_progess / (_count == 0 ? 1 : _count) * 100D, 2);
        }
    }
}
