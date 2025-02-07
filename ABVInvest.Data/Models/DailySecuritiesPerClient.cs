﻿using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Data.Models
{
    public class DailySecuritiesPerClient : BaseEntity<int>
    {
        public DailySecuritiesPerClient()
        {
            this.SecuritiesPerIssuerCollection = [];
        }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }

        public virtual ICollection<SecuritiesPerClient> SecuritiesPerIssuerCollection { get; set; }
    }
}
