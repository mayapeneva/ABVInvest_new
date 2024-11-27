namespace ABVInvest.Common.TestModels
{
    public class PortfolioTestModel
    {
        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public string SecurityIsin { get; set; }

        public string DailySecuritiesPerClientDate { get; set; }

        public decimal Quantity { get; set; }

        public string CurrencyCode { get; set; }

        public decimal AveragePriceBuy { get; set; }

        public decimal TotalPriceBuy { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal TotalMarketPrice { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitPercentage { get; set; }

        public decimal PortfolioShare { get; set; }
    }
}
