using Core.DTOs;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
    }
}