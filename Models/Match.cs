﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TFT_Stats.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        public string RiotMatchID { get; set; }

        //Relationships
        public ICollection<Participant> Participants{ get; set; } = new List<Participant>();

    }
}