using System.ComponentModel.DataAnnotations;

namespace TwinsArtstyle.Infrastructure.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public IList<CartProductCount> CartProductsCount { get; set; } = new List<CartProductCount>();

        //public decimal TotalPrice { get; private set; }
    }
}
