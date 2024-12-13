using Core.Data.DTOs;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
    }
}