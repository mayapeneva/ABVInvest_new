using ABVInvest.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABVInvest.Data.Models
{
    public class Deal : BaseEntity<int>
    {
        public virtual DailyDeals DailyDeals { get; set; }
        public int DailyDealsId { get; set; }

        [Required]
        public DealType DealType { get; set; }

        [Required]
        public virtual Security Security { get; set; }
        public int SecurityId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Coupon { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TotalPriceInBGN { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Fee { get; set; }

        [Required]
        public virtual Currency Currency { get; set; }
        public int CurrencyId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Settlement { get; set; }

        [Required]
        public virtual Market Market { get; set; }
        public int MarketId { get; set; }
    }
}
