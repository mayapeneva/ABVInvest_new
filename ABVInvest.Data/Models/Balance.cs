using System.ComponentModel.DataAnnotations.Schema;

namespace ABVInvest.Data.Models
{
    public class Balance : BaseEntity<int>
    {
        private const string DateTimeParseFormat = "dd/MM/yyyy";

        public Balance()
        {
            this.UsersPortfolio = [];
        }

        public virtual DailyBalance? DaiyBalance { get; set; }

        public string? CurrencyCode { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cash { get; set; }

        [NotMapped]
        public ICollection<SecuritiesPerClient> UsersPortfolio { get; private set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal AllSecuritiesTotalPriceBuy { get; private set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal AllSecuritiesTotalMarketPrice { get; private set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal VirtualProfit { get; private set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal VirtualProfitPercentage { get; private set; }

        public void SetBalanceFigures(ApplicationUser user, DateOnly date)
        {
            this.UsersPortfolio = user.Portfolio.SingleOrDefault(p => p.Date.ToString(DateTimeParseFormat) == date.ToString(DateTimeParseFormat))?.SecuritiesPerIssuerCollection ?? [];

            if (this.UsersPortfolio.Count > 0)
            {
                this.AllSecuritiesTotalPriceBuy = this.UsersPortfolio.Sum(s => s.TotalPriceBuy);
                this.AllSecuritiesTotalMarketPrice = this.UsersPortfolio.Sum(s => s.TotalMarketPrice);

                this.VirtualProfit = this.UsersPortfolio.Sum(s => s.ProfitInBGN);
                this.VirtualProfitPercentage = (this.VirtualProfit * 100) / (this.AllSecuritiesTotalPriceBuy + this.Cash);

                this.CurrencyCode = "BGN";
            }
        }
    }
}