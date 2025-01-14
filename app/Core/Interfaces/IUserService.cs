using Core.Data.DTOs;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> ValidateUser(string username, string password);
    }
}