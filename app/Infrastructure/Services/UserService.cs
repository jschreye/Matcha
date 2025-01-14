using Core.Interfaces;
using Core.Data.Entity;
using Core.Repository;
using Core.Data.DTOs;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUserAsync();
    }
}