using Core.Interfaces;
using Core.Data.DTOs;
using Core.Repository;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository repository;
    public UserService(IUserRepository _repository)
    {
        this.repository = _repository;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await repository.GetAllUserAsync();
    }
}