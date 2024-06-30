using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyHelper.Models.Books
{
    class MyBook : INotifyPropertyChanged
    {
        public string? Author { get; set; }
        public string? Name { get; set; }
        public ushort Pages { get; set; }

        private bool _isReaded;
        public bool IsReaded
        {
            get => _isReaded;
            set
            {
                if (Equals(value, _isReaded)) return;
                _isReaded = value;
                OnPropertyChanged();
            }
        }

        private bool _isReading;
        public bool IsReading
        {
            get => _isReading;
            set
            {
                if(Equals(value, _isReading)) return;
                _isReading = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
