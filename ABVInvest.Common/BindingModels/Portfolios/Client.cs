using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ABVInvest.Common.BindingModels.Portfolios
{
    [XmlType("Client", IncludeInSchema = true)]
    public class Client
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
