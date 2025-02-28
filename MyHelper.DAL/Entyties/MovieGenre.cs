using MyHelper.DAL.Entyties.Base;

namespace MyHelper.DAL.Entyties
{
    public class MovieGenre : Entity
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
