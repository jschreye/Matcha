using Core.Data.Entity;

namespace Core.Interfaces.Services
{
    public interface IChatService
    {
        IReadOnlyList<Message> Messages { get; }
        event Action<Message>? OnMessageReceived;
        void SendMessage(int senderId, int receiverId, string contenu);
    }
}