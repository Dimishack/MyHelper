using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MyHelper.Models.Challenges
{
    internal class Checklist : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int NumberDay { get; set; }
        public DateTime Date { get; set; }
        public DayOfWeek DayWeek => Date.DayOfWeek;

        private bool _check;
        public bool Check
        {
            get => _check; 
            set
            {
                if(Equals(_check, value)) return;
                _check = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? PropertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
}
