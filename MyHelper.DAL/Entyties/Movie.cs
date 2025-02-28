using MyHelper.DAL.Entyties.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHelper.DAL.Entyties
{
    public class Movie : NamedEntity
    {
        public string Producer { get; set; }

        public int ReleaseYear { get; set; }

        public int Format { get; set; }

        public int Status { get; set; }

        public int? Raiting { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ViewingDate { get; set; }

        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
