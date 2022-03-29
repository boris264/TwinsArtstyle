using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TwinsArtstyle.Services.Enums;

namespace TwinsArtstyle.Infrastructure.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public IList<OrderProductCount> OrderProducts { get; set; } = new List<OrderProductCount>();
        public int AddressId { get; set; }

        [ForeignKey(nameof(AddressId))]
        public Address Address { get; set; }

        public decimal Price { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.Confirmed;
    }
}
