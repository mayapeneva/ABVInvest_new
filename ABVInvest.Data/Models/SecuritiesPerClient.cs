using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABVInvest.Data.Models
{
    public class SecuritiesPerClient : BaseEntity<int>
    {
        public virtual DailySecuritiesPerClient DailySecuritiesPerClient { get; set; }
        public int DailySecuritiesPerClientId { get; set; }

        public virtual Security Security { get; set; }
        public int SecurityId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Quantity { get; set; }

        [Required]
        public virtual Currency Currency { get; set; }
        public int CurrencyId { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal AveragePriceBuy { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal TotalPriceBuy => this.Quantity * this.AveragePriceBuy;

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal MarketPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TotalMarketPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Profit { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ProfitInBGN { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ProfitPercentage { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal PortfolioShare { get; set; }
    }
}
