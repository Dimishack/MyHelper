using MyHelper.DAL.Entyties;
using System.ComponentModel;

namespace MyHelper.Models.Targets
{
    class TargetModel(Target target) : INotifyPropertyChanged
    {
        private readonly Target _target = target;

        public bool IsComplete
        {
            get => _target.IsComplete;
            set
            { 
                if (_target.IsComplete == value) return;
                _target.IsComplete = value;
                OnPropertyChanged(nameof(IsComplete));
            }
        }

        public int Id => _target.Id;
        public string Name { get => _target.Name; set => _target.Name = value; }
        public string Note { get => _target.Note; set => _target.Note = value; }


        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
