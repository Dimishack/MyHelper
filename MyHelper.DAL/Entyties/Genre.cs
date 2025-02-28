using MyHelper.DAL.Entyties.Base;
using System.Collections.Generic;

namespace MyHelper.DAL.Entyties
{
    public class Genre : NamedEntity
    {
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
