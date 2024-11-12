using MyHelper.DAL.Entyties.Base;

namespace MyHelper.DAL.Entyties
{
    public class Target : NotedEntity
    {
        public int TargetsGroupId { get; set; }
        public bool IsComplete { get; set; }
        public TargetsGroup TargetsGroup { get; set; }
    }

}
