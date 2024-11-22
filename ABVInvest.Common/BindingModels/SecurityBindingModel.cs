namespace ABVInvest.Common.BindingModels
{
    public struct SecurityBindingModel
    {
        public string Issuer { get; set; }

        public string ISIN { get; set; }

        public string BfbCode { get; set; }

        public string Currency { get; set; }
    }
}
