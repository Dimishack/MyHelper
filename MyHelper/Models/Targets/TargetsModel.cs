using MyHelper.DAL.Entyties;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyHelper.Models.Targets
{
    internal class TargetsModel : INotifyPropertyChanged
    {
        private TargetsGroup _targetsGroup;

        public ObservableCollection<TargetModel> Targets { get; } = [];

        public TargetsModel(TargetsGroup targetsGroup)
        {
            _targetsGroup = targetsGroup;
            Targets.CollectionChanged += Targets_CollectionChanged;

            foreach (var target in targetsGroup.Targets)
            {
                var newTarget = new TargetModel(target);
                newTarget.PropertyChanged += NewTarget_PropertyChanged;
                Targets.Add(newTarget);
            }
        }

        private void Targets_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null && e.NewItems[0] is TargetModel target)
                        if (target.IsComplete) CompletedTargetsCount++;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    CompletedTargetsCount--;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    CompletedTargetsCount = 0;
                    break;
                default:
                    break;
            }
        }

        private void NewTarget_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is TargetModel target && e.PropertyName == nameof(target.IsComplete))
                CompletedTargetsCount += target.IsComplete ? 1 : -1;
        }

        public int Id => _targetsGroup.Id;
        public string Name
        {
            get => _targetsGroup.Name;
            set
            {
                if (value == _targetsGroup.Name) return;
                _targetsGroup.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public uint Year { get => _targetsGroup.Year; set => _targetsGroup.Year = value; }

        private int _completedTargetsCount;
        public int CompletedTargetsCount
        {
            get => _completedTargetsCount;
            private set
            {
                if (value == _completedTargetsCount) return;
                _completedTargetsCount = value;
                OnPropertyChanged(nameof(CompletedTargetsCount));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
