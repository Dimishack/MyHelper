using MyHelper.Interfaces;

namespace MyHelper.DAL.Entyties.Base
{
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
    }
}
