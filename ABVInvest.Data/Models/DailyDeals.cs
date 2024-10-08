﻿using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Data.Models
{
    public class DailyDeals : BaseEntity<int>
    {
        public DailyDeals()
        {
            this.Deals = [];
        }

        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public virtual ICollection<Deal> Deals { get; set; }
    }
}