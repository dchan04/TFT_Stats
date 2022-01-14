using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TFT_Stats.Models.ViewModel
{
    public class CompanionVM
    {
        [Key]
        public int ID { get; set; }
        public String Name { get; set; }

        public String Species { get; set; }

        public int Level1 { get; set; }

        public int Level2 { get; set; }

        public int Level3 { get; set; }

        public int Count { get; set; }

        public String ImgUrl { get; set; }

    }
}
