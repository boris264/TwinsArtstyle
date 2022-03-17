using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Infrastructure.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public IList<Product> Products { get; set; } = new List<Product>();

    }
}