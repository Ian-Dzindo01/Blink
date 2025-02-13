using Blink.Models;
using Microsoft.AspNetCore.SignalR;

namespace Blink.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            var chatMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.User(receiverId).SendAsync("ReceiveMessage", chatMessage);
        }
    }
}
