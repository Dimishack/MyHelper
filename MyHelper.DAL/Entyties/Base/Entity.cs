using MyHelper.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MyHelper.DAL.Entyties.Base
{
    public abstract class Entity : IEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
