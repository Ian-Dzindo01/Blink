using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blink.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey("Sender")]
        public string SenderId { get; set; }
        public User Sender { get; set; }

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }
        public User Receiver { get; set; }
    }
}