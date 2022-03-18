using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwinsArtstyle.Infrastructure.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        [StringLength(100)]

        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Body { get; set; }
    }
}
