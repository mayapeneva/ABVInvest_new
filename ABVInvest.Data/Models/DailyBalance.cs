using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Data.Models
{
    public class DailyBalance : BaseEntity<int>
    {
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public virtual Balance Balance { get; set; }
        public int BalanceId { get; set; }
    }
}
