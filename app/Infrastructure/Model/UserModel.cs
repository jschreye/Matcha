using Core.Repository;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.DTOs;

namespace Infrastructure.Model
{
    public class UserModel : IUserRepository
    {
        private readonly string? _connectionString;
        public UserModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<UserDto>> GetAllUserAsync()
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
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }

            return users;
        }
    }
}
