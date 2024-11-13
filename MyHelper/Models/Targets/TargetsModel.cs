using MyHelper.DAL.Entyties;
using System.Collections.ObjectModel;

namespace MyHelper.Models.Targets
{
    internal class TargetsModel
    {
        private TargetsGroup _targetsGroup;
        public ObservableCollection<TargetModel> Targets { get; } = [];

        public TargetsModel(TargetsGroup targetsGroup)
        {
            _targetsGroup = targetsGroup;

            foreach (var target in targetsGroup.Targets)
                Targets.Add(new TargetModel(target));
        }

        public string Name { get => _targetsGroup.Name; set => _targetsGroup.Name = value; }
        public uint Year { get => _targetsGroup.Year; set => _targetsGroup.Year = value;}

    }
}
