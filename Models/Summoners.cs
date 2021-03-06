using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TFT_Stats.Models
{
    public class Summoners
    {
        [Key]
        public int Id { get; set; }

        public string RiotSummonerId { get; set; }

        public string Puuid { get; set;}

        //Relationships
        public ICollection<Match> Matches { get; set; } = new List<Match>();

    }
}
