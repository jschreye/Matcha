using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.Data.DTOs;
using Core.Data.Entity;
using Core.Interfaces.Repository;

namespace Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
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

        public async Task<User?> GetByEmail(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand(@"
                SELECT 
                    id, firstname, lastname, username, email, 
                    password_hash, isactive, activationtoken, passwordresettoken, created_at 
                FROM users 
                WHERE email = @Email", connection);
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
                    PasswordHash = reader.IsDBNull(reader.GetOrdinal("password_hash"))
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("password_hash")),
                    IsActive = !reader.IsDBNull(reader.GetOrdinal("isactive")) 
                                    && reader.GetBoolean(reader.GetOrdinal("isactive")),
                    ActivationToken = reader.IsDBNull(reader.GetOrdinal("activationtoken"))
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("activationtoken")),
                    PasswordResetToken = reader.IsDBNull(reader.GetOrdinal("passwordresettoken"))
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("passwordresettoken")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }
            return null;
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand(@"
                SELECT 
                    id, firstname, lastname, username, email, 
                    password_hash, isactive, activationtoken, created_at 
                FROM users 
                WHERE username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

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
                    PasswordHash = reader.IsDBNull(reader.GetOrdinal("password_hash"))
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("password_hash")),
                    IsActive = !reader.IsDBNull(reader.GetOrdinal("isactive"))
                                    && reader.GetBoolean(reader.GetOrdinal("isactive")),
                    ActivationToken = reader.IsDBNull(reader.GetOrdinal("activationtoken")) 
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("activationtoken")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }
            return null;
        }

        public async Task Add(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO users 
                    (firstname, lastname, username, email, password_hash, isactive, activationtoken, created_at) 
                VALUES 
                    (@Firstname, @Lastname, @Username, @Email, @PasswordHash, @IsActive, @ActivationToken, @CreatedAt)";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Firstname", user.Firstname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Lastname", user.Lastname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Username", user.Username ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@ActivationToken", user.ActivationToken ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE users 
                SET 
                    firstname = @Firstname,
                    lastname = @Lastname,
                    username = @Username,
                    email = @Email,
                    password_hash = @PasswordHash,
                    isactive = @IsActive,
                    activationtoken = @ActivationToken,
                    passwordresettoken = @PasswordResetToken
                WHERE id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Firstname", user.Firstname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Lastname", user.Lastname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Username", user.Username ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@ActivationToken", user.ActivationToken ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordResetToken", user.PasswordResetToken ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Id", user.Id);

            await command.ExecuteNonQueryAsync();
        }
        public async Task<string?> GetPasswordHashByUsernameAsync(string username)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT password_hash FROM users WHERE username=@username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);

            var dbPasswordHash = await command.ExecuteScalarAsync() as string;
            return dbPasswordHash;
        }
    }
}
