using MyHelper.DAL.Entyties;
using MyHelper.Models.Base;
using System.Collections.ObjectModel;

namespace MyHelper.Models.Targets
{
    internal class TargetsModel : BaseModel
    {
        private readonly TargetsGroup _targetsGroup;

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

        public TargetsModel(TargetsGroup targetsGroup)
        {
            _targetsGroup = targetsGroup;

            foreach (var target in targetsGroup.Targets)
            {
                var newTarget = new TargetModel(target);
                Targets.Add(newTarget);
            }
        }
    }
}
