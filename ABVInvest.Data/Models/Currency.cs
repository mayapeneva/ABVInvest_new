using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Data.Models
{
    public class Currency : BaseEntity<int>
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$")]
        public string Code { get; set; }
    }
}
