using ABVInvest.Common.BindingModels.Portfolios;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ABVInvest.Common.BindingModels.Deals
{
    [XmlType("New", IncludeInSchema = true)]
    public struct DealRowBindingModel
    {
        [XmlElement("ABVClient")]
        [Required]
        public AppClient Client { get; set; }

        [XmlElement("Instrument")]
        [Required]
        public Instrument Instrument { get; set; }

        [XmlElement("DealData")]
        [Required]
        public DealData DealData { get; set; }
    }
}
