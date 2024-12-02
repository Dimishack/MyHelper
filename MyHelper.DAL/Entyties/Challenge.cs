using MyHelper.DAL.Entyties.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHelper.DAL.Entyties
{
    public class Challenge : NotedEntity
    {
        public bool InProgress { get; set; }

        [Column(TypeName ="date")]
        public DateTime? Start { get; set; }

        [Column(TypeName = "date")]
        public DateTime? End { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? Duration { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string? Regularity { get; set; }

        public int? AdditionalRegularity { get; set; }

        public virtual ICollection<Check> CheckList { get; set; } = new List<Check>();
    }
}
