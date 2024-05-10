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
        private int _progress;
        /// <summary>Количества выполненных чеков</summary>
        public int Progress
        {
            get => _progress;
            set => Set(ref _progress, value);
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

        #region SelectedDate : Checklist - Выбранная дата

        ///<summary>Выбранная дата</summary>
        private Checklist? _selectedDate;

        ///<summary>Выбранная дата</summary>
        public Checklist? SelectedDate { get => _selectedDate; set => Set(ref _selectedDate, value); }

        #endregion


        public ChecklistChallengeViewModel(MyChallenge checklist)
        {
            _title = checklist.Challenge;
            _checklists = new(checklist.Checklist!);
            _count = _checklists.Count;
            _progress = _checklists.Where(i => i.Check).Count();
            _procent = double.Round((double)_progress / (_count == 0 ? 1 : _count) * 100D, 2);
            _checklists.ListChanged += Checklists_ListChanged;
            _selectedDate = _checklists.FirstOrDefault(c => c.Date == DateTime.Today);
        }

        private void Checklists_ListChanged(object? sender, ListChangedEventArgs e)
        {
            Count = _checklists?.Count ?? 0;
            Progress = _checklists?.Where(i => i.Check).Count() ?? 0;
            Procent = double.Round((double)_progress / (_count == 0 ? 1 : _count) * 100D, 2);
        }
    }
}
