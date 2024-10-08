using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Data.Models
{
    public abstract class BaseEntity<T>
    {
        [Required]
        public T Id { get; set; }
    }
}
