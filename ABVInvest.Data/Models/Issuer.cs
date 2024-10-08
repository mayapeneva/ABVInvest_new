using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Data.Models
{
    public class Issuer : BaseEntity<int>
    {
        public Issuer()
        {
            this.Securities = [];
        }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public virtual ICollection<Security> Securities { get; set; }
    }
}
