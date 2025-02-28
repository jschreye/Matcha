using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task SaveMessageAsync(Message message);
        Task<List<Message>> GetConversationAsync(int userId1, int userId2);
    }
}