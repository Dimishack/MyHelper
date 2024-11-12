using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHelper.DAL.Entyties.Base
{
    public abstract class NamedEntity : Entity
    {
        [Required]
        [Column(TypeName = "varchar(300)")]
        public string Name { get; set; }
    }
}
