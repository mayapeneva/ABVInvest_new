using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ABVInvest.Common.BindingModels.Portfolios
{
    [XmlType("ABVClient", IncludeInSchema = true)]
    public struct AppClient
    {
        [XmlElement("CDNumber")]
        [Required]
        [RegularExpression(@"^[A-Z0-9]{5}$|^[A-Z0-9]{10}$")]
        public string CDNNumber { get; set; }

        [XmlElement("Name")]
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}
