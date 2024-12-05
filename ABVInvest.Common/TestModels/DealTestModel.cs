using ABVInvest.Data.Models.Enums;

namespace ABVInvest.Common.TestModels
{
    public class DealTestModel
    {
        public DealType DealType { get; set; }

        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public DateOnly DailyDealsDate { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Coupon { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Fee { get; set; }

        public string CurrencyCode { get; set; }

        public DateOnly Settlement { get; set; }

        public string MarketName { get; set; }
    }
}
