namespace ABVInvest.Common.TestModels
{
    public class BalanceTestModel
    {
        public string CurrencyCode { get; set; }

        public decimal Cash { get; set; }

        public decimal AllSecuritiesTotalPriceBuy { get; set; }

        public decimal AllSecuritiesTotalMarketPrice { get; set; }

        public decimal VirtualProfit { get; set; }

        public decimal VirtualProfitPercentage { get; set; }
    }
}
