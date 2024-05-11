using MyHelper.Models.Challenges;
using MyHelper.ViewModels.Base;
using System;
using System.ComponentModel;

namespace MyHelper.ViewModels
{
    class ChecklistChallengeViewModel : ViewModel
    {

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

        #region CheckLists : BindingList<Checklist> - Чек-лист
        /// <summary>Чек-лист</summary>
        private BindingList<Checklist> _checklists;
        /// <summary>Чек-лист</summary>
        public BindingList<Checklist> Checklists
        {
            get => _checklists;
            set => Set(ref _checklists, value);
        }
        #endregion

        #region Progress : int - Количества выполненных чеков

        /// <summary>Количества выполненных чеков</summary>
        public int Progress => _checklists.Where(i => i.Check).Count();
        #endregion

        #region Procent : double - Процент выполненных чеков

        /// <summary>Процент выполненных чеков</summary>
        public double Procent => (double)Progress / (_checklists.Count == 0 ? 1 : _checklists.Count);
        #endregion

        #region SelectedDate : Checklist - Выбранная дата

        ///<summary>Выбранная дата</summary>
        private Checklist? _selectedDate;

        ///<summary>Выбранная дата</summary>
        public Checklist? SelectedDate { get => _selectedDate; set => Set(ref _selectedDate, value); }

        #endregion

        #region OffsetLimeGreenColor : double - Местоположение лаймового цвета в ProgressBar'e

        /// <summary>Местоположение лаймового цвета в ProgressBar'e</summary>
        public double OffsetLimeGreenColor => 2.0 - Procent;

        #endregion 

        public ChecklistChallengeViewModel(MyChallenge checklist)
        {
            _title = checklist.Challenge;
            _checklists = new(checklist.Checklist!);
            _checklists.ListChanged += Checklists_ListChanged;
            _selectedDate = _checklists.FirstOrDefault(c => c.Date == DateTime.Today);
        }

        private void Checklists_ListChanged(object? sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(Procent));
            OnPropertyChanged(nameof(OffsetLimeGreenColor));
        }
    }
}
