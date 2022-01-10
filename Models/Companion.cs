using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TFT_Stats.Models
{
    public class Companion
    {
        [Key]
        public int Id { get; set; }
        public int SkinID { get; set; }

        public int? Level { get; set; }

        public string RiotCompanionID { get; set; }

        public string Name { get; set; }

        public string Species { get; set; }

        public string ImgPath { get; set; }
    }
}
