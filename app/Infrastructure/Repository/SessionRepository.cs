using MySql.Data.MySqlClient;
using Core.Interfaces.Services;
using Core.Interfaces.Repository;

namespace Infrastructure.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly string _connectionString;

        public SessionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateSessionAsync(int userId, string sessionToken, DateTime expiresAt)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO sessions (user_id, session_token, expires_at) VALUES (@userId, @sessionToken, @expiresAt)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@sessionToken", sessionToken);
            command.Parameters.AddWithValue("@expiresAt", expiresAt);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> ValidateSessionAsync(string sessionToken)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM sessions WHERE session_token = @sessionToken AND expires_at > NOW()";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@sessionToken", sessionToken);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        public async Task DeleteSessionAsync(string sessionToken)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM sessions WHERE session_token = @sessionToken";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@sessionToken", sessionToken);

            await command.ExecuteNonQueryAsync();
        }
        public async Task<bool> HasActiveSessionAsync(int userId)
        {
            const string sql = @"
                SELECT EXISTS(
                    SELECT 1
                    FROM sessions
                    WHERE user_id = @userId
                    AND expires_at > NOW()
                )";
            await using var cn = new MySqlConnection(_connectionString);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@userId", userId);
            var exists = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(exists) == 1;
        }
        public async Task DeleteSessionsByUserIdAsync(int userId)
        {
            const string sql = "DELETE FROM sessions WHERE user_id = @userId;";
            await using var cn = new MySqlConnection(_connectionString);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@userId", userId);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}