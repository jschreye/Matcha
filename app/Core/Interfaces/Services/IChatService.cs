using Core.Data.Entity;

namespace Core.Interfaces.Services
{
    public interface IChatService
    {
        IReadOnlyList<Message> Messages { get; }
        event Action<Message>? OnMessageReceived;
        Task SendMessageAsync(int senderId, int receiverId, string contenu);
        Task<List<Message>> LoadConversationAsync(int userId1, int userId2);
    }
}