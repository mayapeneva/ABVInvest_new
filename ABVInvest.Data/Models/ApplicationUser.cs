using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Portfolio = [];
            Deals = [];
            Balances = [];
        }

        [RegularExpression(@"^\d{5}$")]
        public string? PIN { get; set; }

        [DataType(DataType.Text)]
        [MinLength(4)]
        public string? FullName { get; set; }

        public virtual ICollection<DailySecuritiesPerClient> Portfolio { get; set; }

        public virtual ICollection<DailyDeals> Deals { get; set; }

        public virtual ICollection<DailyBalance> Balances { get; set; }

        [Required]
        public int BalanceId { get; set; }
    }
}
