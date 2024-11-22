using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ABVInvest.Common.BindingModels.Portfolios
{
    [XmlType("AccountData")]
    public struct AccountData
    {
        [XmlElement("Quantity")]
        [Required]
        public string Quantity { get; set; }

        [XmlElement("OpenPrice")]
        [Required]
        public string OpenPrice { get; set; }

        [XmlElement("MarketPrice")]
        [Required]
        public string MarketPrice { get; set; }

        [XmlElement("MarketValue")]
        [Required]
        public string MarketValue { get; set; }

        [XmlElement("MarketDate")]
        [Required]
        public string MarketDate { get; set; }

        [XmlElement("Result")]
        [Required]
        public string Result { get; set; }

        [XmlElement("ResultBGN")]
        [Required]
        public string ResultBGN { get; set; }
    }
}
