using MyHelper.DAL.Entyties.Base;
using System.Collections.Generic;

namespace MyHelper.DAL.Entyties
{
    public class TargetsGroup : NamedEntity
    {
        public uint Year { get; set; }
        public virtual ICollection<Target> Targets { get; set; } = new List<Target>();
    }

}
