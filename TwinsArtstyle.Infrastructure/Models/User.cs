using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsArtstyle.Infrastructure.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public Guid CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }

        public IList<Address> Addresses { get; set; } = new List<Address>();

        public IList<Order> Orders { get; set; } = new List<Order>();
    }
}
