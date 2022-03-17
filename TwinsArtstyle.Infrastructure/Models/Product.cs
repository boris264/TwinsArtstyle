using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsArtstyle.Infrastructure.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        [Range(0, 200)]
        public decimal Price { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }
    }
}
