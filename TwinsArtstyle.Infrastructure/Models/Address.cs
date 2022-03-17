using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Infrastructure.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string AddressText { get; set; }
    }
}
