using MyHelper.DAL.Entyties.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyHelper.DAL.Entyties
{
    public class Check : Entity
    {
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public int NumberDay { get; set; }
        public bool Checked { get; set; }
        public int ChallengeId { get; set; }
        public Challenge Challenge { get; set; }
    }
}
