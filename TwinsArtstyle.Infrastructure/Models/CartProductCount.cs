using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsArtstyle.Infrastructure.Models
{
    public class CartProductCount
    {
        [Key]
        public Guid CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Guid ProductId { get; set; }

        public Product Product { get; set; }
        
        public int Count { get; set; }
    }
}
