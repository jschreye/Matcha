using Core.Data.DTOs;
using Core.Data.Entity;

namespace Core.Repository
{
    public interface IUserRepository
    {
        Task<List<UserDto>>GetAllUserAsync();
        Task<User> GetByEmail(string email);
        Task Add(User user);
        Task Update(User user);
        Task<User> FindByUsernameAsync(string username);
    }
}