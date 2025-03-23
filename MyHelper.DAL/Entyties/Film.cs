using MyHelper.DAL.Entyties.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHelper.DAL.Entyties
{
    public class Film : NamedEntity
    {
        public string Producer { get; set; }

        public int ReleaseYear { get; set; }

        public int Format { get; set; }

        public int Status { get; set; }

        public int Raiting { get; set; } = 1;

        [Column(TypeName = "date")]
        public DateTime? ViewingDate { get; set; }

        public virtual ICollection<FilmGenre> FilmGenres { get; set; }
    }
}
