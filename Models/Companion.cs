using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFT_Stats.Models
{
    public class Companion
    {
        [Key]
        public int Id { get; set; }

        public string RiotCompanionID { get; set; }

        public int SkinID { get; set; }

    }
}
