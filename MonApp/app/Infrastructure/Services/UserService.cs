using Core.Interfaces;
using Core.Models;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.DTOs;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly string _connectionString;

    public UserService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = new List<UserDto>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand("SELECT id, username, email, created_at FROM users", connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new UserDto
            {
                Id = reader.GetInt32("id"),
                Username = reader.GetString("username"),
                Email = reader.GetString("email"),
                CreatedAt = reader.GetDateTime("created_at")
            });
        }

        return users;
    }
}