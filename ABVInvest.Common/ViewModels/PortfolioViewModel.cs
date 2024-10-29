namespace ABVInvest.Common.ViewModels
{
    public class PortfolioViewModel
    {
        public string SecurityIssuerName { get; set; }

        public string SecurityBfbCode { get; set; }

        public string SecurityIsin { get; set; }

        public string DailySecuritiesPerClientDate { get; set; }

        public string Quantity { get; set; }

        public string CurrencyCode { get; set; }

        public string AveragePriceBuy { get; set; }

        public string TotalPriceBuy { get; set; }

        public string MarketPrice { get; set; }

        public string TotalMarketPrice { get; set; }

        public decimal Profit { get; set; }

        public string ProfitPercentage { get; set; }

        public string PortfolioShare { get; set; }
    }
}
