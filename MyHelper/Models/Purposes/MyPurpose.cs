using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyHelper.Models.Purposes
{
    internal class MyPurpose : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if(Equals(value, _isCompleted)) return;
                _isCompleted = value;
                OnPropertyChanged();
            }
        }
        public string? Purpose { get; set; }
        public string? Note { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}