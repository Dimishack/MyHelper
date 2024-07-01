using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyHelper.Models.MyTasks
{
    internal class MyTask : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string? Task { get; set; }

        private bool _prompt;
        public bool Prompt
        {
            get => _prompt;
            set
            {
                if (Equals(_prompt, value)) return;
                _prompt = value;
                OnPropertyChanged();
            }
        }

        private bool _important;
        public bool Important
        {
            get => _important;
            set
            {
                if (Equals(_important, value)) return;
                _important = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Term { get; set; }

        public string? Group { get; set; }

        public string? Note { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
