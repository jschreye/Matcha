using Core.Data.DTOs;

namespace Core.Repository
{
    public interface IUserRepository
    {
        Task<List<UserDto>>GetAllUserAsync();
    }
}