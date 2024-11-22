using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ABVInvest.Common.BindingModels.Deals
{
    [XmlType("DealData")]
    public struct DealData
    {
        [XmlElement("Operation")]
        [Required]
        [RegularExpression("^SELL|BUY$")]
        public string Operation { get; set; }

        [XmlElement("DeliveryDate")]
        [Required]
        public string DeliveryDate { get; set; }

        [XmlElement("StockExchangeMIC")]
        [Required]
        [RegularExpression("[A-Z]{4}")]
        public string StockExchangeMIC { get; set; }

        [XmlElement("ShareCount")]
        [Required]
        public string ShareCount { get; set; }

        [XmlElement("SinglePrice")]
        [Required]
        public string SinglePrice { get; set; }

        [XmlElement("Coupon")]
        [Required]
        public string Coupon { get; set; }

        [XmlElement("DealAmountInShareCurrency")]
        [Required]
        public string DealAmountInShareCurrency { get; set; }

        [XmlElement("PaymentCurrency")]
        [Required]
        public string PaymentCurrency { get; set; }

        [XmlElement("DealAmountInPaymentCurrency")]
        [Required]
        public string DealAmountInPaymentCurrency { get; set; }

        [XmlElement("CommissionInPaymentCurrency")]
        [Required]
        public string CommissionInPaymentCurrency { get; set; }
    }
}
