using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFT_Stats.Models
{
    public class Participant
    {
        [Key]
        public string Id { get; set; }

        public Companion Companion { get; set; }

        public int MatchID { get; set; }
        [ForeignKey("MatchID")]
        public Match Match { get; set; }
    }
}
