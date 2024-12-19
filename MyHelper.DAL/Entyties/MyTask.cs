using MyHelper.DAL.Entyties.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHelper.DAL.Entyties
{
    public class MyTask : NotedEntity
    {
        [Column(TypeName = "varchar(20)")]
        public string Group { get; set; }

        [Column(TypeName = "date")]
        public DateTime End { get; set; }

        public bool Prompt { get; set; }

        public bool Important { get; set; }
    }
}
