using Core.Data.Entity;
using Core.Data.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task SaveMessageAsync(Message message);
        Task<List<Message>> GetConversationAsync(int userId1, int userId2);
        Task<List<ConversationDto>> GetConversationsForUserAsync(int userId);
    }
}