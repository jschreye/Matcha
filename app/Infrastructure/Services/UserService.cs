using Core.Interfaces;
using Core.Data.Entity;
using Core.Repository;
using Core.Data.DTOs;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly string _connectionString;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionRepository _sessionRepository;
    public UserService(IConfiguration config, IUserRepository userRepository, IPasswordHasher passwordHasher, ISessionRepository sessionRepository)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _sessionRepository = sessionRepository;
    }
    
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUserAsync();
    }

    public async Task<bool> ValidateUser(string username, string password)
    {
        using(var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            // Utilisation de la colonne 'password_hash' telle que définie dans la table
            var query = "SELECT password_hash FROM users WHERE username=@username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);

            var dbPasswordHash = await command.ExecuteScalarAsync() as string;
            if (string.IsNullOrEmpty(dbPasswordHash))
            {
                return false;
            }
            
            // Vérifie le mot de passe fourni avec le haché récupéré
            return _passwordHasher.VerifyPassword(password, dbPasswordHash);
        }
    }

    public async Task CreateSessionAsync(int userId, string sessionToken, DateTime expiresAt)
    {
        await _sessionRepository.CreateSessionAsync(userId, sessionToken, expiresAt);
    }

    public async Task<bool> ValidateSessionAsync(string sessionToken)
    {
        return await _sessionRepository.ValidateSessionAsync(sessionToken);
    }

    public async Task DeleteSessionAsync(string sessionToken)
    {
        await _sessionRepository.DeleteSessionAsync(sessionToken);
    }
}