using Microsoft.AspNetCore.Identity;

namespace Blink.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Message> SentMessages { get; set; } = new();

        public List<Message> ReceivedMessages { get; set; } = new();
    }
}