using Microsoft.AspNetCore.SignalR;
using ECommerce.Models;

namespace ECommerce.Hubs
{
    public class ChatHub : Hub
    {
        // In-memory storage for chat messages (in production, use database)
        private static List<ChatMessageModel> ChatMessages = new();
        private static int ConnectedUsers = 0;

        public async Task SendMessage(string userId, string userName, string message, string? profileImage = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var chatMessage = new ChatMessageModel
            {
                Id = ChatMessages.Count + 1,
                UserId = userId,
                UserName = userName,
                Message = message.Trim(),
                Timestamp = DateTime.UtcNow,
                UserProfileImage = profileImage
            };

            ChatMessages.Add(chatMessage);
            Console.WriteLine($"[Chat] Message from {userName}: {message}");

            // Send message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", chatMessage);
        }

        public async Task GetChatHistory()
        {
            Console.WriteLine($"[Chat] Getting chat history - {ChatMessages.Count} messages");
            // Send chat history to the client who requested it
            await Clients.Caller.SendAsync("ChatHistory", ChatMessages);
        }

        public override async Task OnConnectedAsync()
        {
            ConnectedUsers++;
            Console.WriteLine($"[Chat Hub] User connected. Total users: {ConnectedUsers}");
            
            await Clients.All.SendAsync("UserConnected", new
            {
                message = $"A user connected. Total users online: {ConnectedUsers}",
                userCount = ConnectedUsers
            });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedUsers = Math.Max(0, ConnectedUsers - 1);
            Console.WriteLine($"[Chat Hub] User disconnected. Total users: {ConnectedUsers}");
            
            await Clients.All.SendAsync("UserDisconnected", new
            {
                message = $"A user disconnected. Total users online: {ConnectedUsers}",
                userCount = ConnectedUsers
            });

            await base.OnDisconnectedAsync(exception);
        }
    }
}
