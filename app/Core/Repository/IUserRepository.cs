using Core.Data.DTOs;
using Core.Data.Entity;

namespace Core.Repository
{
    public interface IUserRepository
    {
        Task<List<UserDto>>GetAllUserAsync();
        Task AddUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task UpdateUserAsync(User user);
    }
}