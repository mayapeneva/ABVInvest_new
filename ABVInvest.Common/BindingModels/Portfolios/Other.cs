using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ABVInvest.Common.BindingModels.Portfolios
{
    [XmlType("Other")]
    public class Other
    {
        [XmlElement("YieldPercent")]
        [Required]
        public string YieldPercent { get; set; }

        [XmlElement("RelativePart")]
        [Required]
        public string RelativePart { get; set; }
    }
}
