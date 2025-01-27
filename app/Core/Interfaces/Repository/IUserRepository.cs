using Core.Data.DTOs;
using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<List<UserDto>>GetAllUserAsync();
        Task<User?> GetByEmail(string email);
        Task Add(User user);
        Task Update(User user);
        Task<User?> FindByUsernameAsync(string username);
        Task<string?> GetPasswordHashByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task UpdateUserAsync(User user);
    }
}