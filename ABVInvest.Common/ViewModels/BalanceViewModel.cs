namespace ABVInvest.Common.ViewModels
{
    public class BalanceViewModel
    {
        public string CurrencyCode { get; set; }

        public string Cash { get; set; }

        public string AllSecuritiesTotalPriceBuy { get; set; }

        public string AllSecuritiesTotalMarketPrice { get; set; }

        public string VirtualProfit { get; set; }

        public string VirtualProfitPercentage { get; set; }
    }
}
