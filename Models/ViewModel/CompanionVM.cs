using Microsoft.AspNetCore.Mvc.Rendering;
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

        public float Level1 { get; set; }

        public float Level2 { get; set; }

        public float Level3 { get; set; }

        public int Count { get; set; }

        public String ImgUrl { get; set; }

    }
}
