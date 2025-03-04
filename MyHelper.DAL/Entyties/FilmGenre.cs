using MyHelper.DAL.Entyties.Base;

namespace MyHelper.DAL.Entyties
{
    public class FilmGenre : Entity
    {
        public int FilmId { get; set; }
        public virtual Film Film { get; set; }
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }
    }
}
