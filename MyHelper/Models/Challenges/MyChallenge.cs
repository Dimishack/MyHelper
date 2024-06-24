using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyHelper.Models.Challenges
{
    internal class MyChallenge : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private bool _isProgress;
        public bool IsProgress
        {
            get => _isProgress;
            set
            {
                if(Equals(value, _isProgress)) return;
                _isProgress = value;
                OnPropertyChanged();
            }
        }
        public string? Challenge { get; set; }

        public string? Duration { get; set; }

        public DateTime? DateStartProgressing { get; set; }

        public string? Note { get; set; }

        public IList<Checklist>? Checklist { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
