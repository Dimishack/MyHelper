using MyHelper.DAL.Entyties;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyHelper.Models.Targets
{
    internal class TargetsModel : INotifyPropertyChanged, IDisposable
    {
        private readonly TargetsGroup _targetsGroup;
        private bool _disposed = false;

        public ObservableCollection<TargetModel> Targets { get; } = [];

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
                if (value == _completedTargetsCount || value < 0) return;
                _completedTargetsCount = value;
                OnPropertyChanged(nameof(CompletedTargetsCount));
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(OffsetOfCompleted));
                OnPropertyChanged(nameof(Procent));
            }
        }

        public double Progress => (double)CompletedTargetsCount / (Targets.Count > 0 ? Targets.Count : 1);

        public double OffsetOfCompleted => 2.0 - Progress;

        public double Procent => Math.Round(Progress * 100.0, 2);

        private void Targets_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            bool isChange = true;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null && e.NewItems[0] is TargetModel target)
                    {
                        target.PropertyChanged += NewTarget_PropertyChanged;
                        if (target.IsComplete)
                        {
                            CompletedTargetsCount++;
                            isChange = false;
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null && e.OldItems[0] is TargetModel oldTarget)
                        oldTarget.PropertyChanged -= NewTarget_PropertyChanged;
                    CompletedTargetsCount--;
                    isChange = false;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    CompletedTargetsCount = 0;
                    isChange = false;
                    break;
                default:
                    break;
            }
            if (isChange)
            {
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(OffsetOfCompleted));
                OnPropertyChanged(nameof(Procent));
            }
        }

        private void NewTarget_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is TargetModel target && e.PropertyName == nameof(target.IsComplete))
            {
                ChangedTargetId = target.Id;
                CompletedTargetsCount += target.IsComplete ? 1 : -1;
            }
        }

        public int? ChangedTargetId { get; private set; }

        public TargetsModel(TargetsGroup targetsGroup)
        {
            _targetsGroup = targetsGroup;
            Targets.CollectionChanged += Targets_CollectionChanged;

            foreach (var target in targetsGroup.Targets)
            {
                var newTarget = new TargetModel(target);
                Targets.Add(newTarget);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Targets.CollectionChanged -= Targets_CollectionChanged;
                    foreach (var target in Targets)
                        target.PropertyChanged -= NewTarget_PropertyChanged;
                    Targets.Clear();
                }
                _disposed = true;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
