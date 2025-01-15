using Core.Repository;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.Data.DTOs;
using Core.Data.Entity;

namespace Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string? _connectionString;
        public UserRepository(IConfiguration configuration)
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
        public async Task<User> GetByEmail(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT id, firstname, lastname, username, email, password_hash, created_at FROM users WHERE email = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Firstname = reader.GetString(reader.GetOrdinal("firstname")),
                    Lastname = reader.GetString(reader.GetOrdinal("lastname")),                   
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }
            return null;
        }

        public async Task Add(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand("INSERT INTO users (firstname, lastname, username, email, password_hash) VALUES (@Firstname, @Lastname, @Username, @Email, @PasswordHash)", connection);
            command.Parameters.AddWithValue("@Firstname", user.Firstname); 
            command.Parameters.AddWithValue("@Lastname", user.Lastname);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            await command.ExecuteNonQueryAsync();
        }
    }
}
