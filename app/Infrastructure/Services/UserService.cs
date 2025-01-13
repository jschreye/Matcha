using Core.Interfaces;
using Core.Data.Entity;
using Core.Repository;
using Core.Data.DTOs;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUserAsync();
    }
    public async Task<bool> RegisterUser(RegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetByEmail(registerDto.Email);
        if (existingUser != null)
            throw new Exception("Cet e-mail est déjà utilisé.");

        var passwordHash = _passwordHasher.HashPassword(registerDto.Password);
        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash
        };

        await _userRepository.Add(user);
        return true;
    }
}