using Core.Interfaces.Repository;
using Core.Data.Entity;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
   public class MatchRepository : IMatchRepository
   {
        private readonly string _connectionString;
        public MatchRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
        }
        public async Task CreateMatchAsync(int userId1, int userId2)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // Toujours user1 < user2
            var ordered = new[] { userId1, userId2 }.OrderBy(i => i).ToArray();

            var query = @"
                INSERT INTO matches (user1_id, user2_id, matched_at, is_active)
                VALUES (@u1, @u2, NOW(), TRUE)
                ON DUPLICATE KEY UPDATE
                    is_active = VALUES(is_active),
                    matched_at = VALUES(matched_at);
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@u1", ordered[0]);
            cmd.Parameters.AddWithValue("@u2", ordered[1]);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteMatchAsync(int userId1, int userId2)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var ordered = new[] { userId1, userId2 }.OrderBy(i => i).ToArray();

            var query = @"
                UPDATE matches
                SET is_active = FALSE
                WHERE user1_id = @u1 AND user2_id = @u2;
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@u1", ordered[0]);
            cmd.Parameters.AddWithValue("@u2", ordered[1]);
            await cmd.ExecuteNonQueryAsync();
        }
        
        public async Task<List<int>> GetMatchedUserIdsAsync(int userId)
        {
            var matchedUserIds = new List<int>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    CASE 
                        WHEN user1_id = @userId THEN user2_id
                        ELSE user1_id
                    END AS matched_user_id
                FROM matches
                WHERE (user1_id = @userId OR user2_id = @userId) AND is_active = TRUE";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                matchedUserIds.Add(reader.GetInt32(reader.GetOrdinal("matched_user_id")));
            }

            return matchedUserIds;
        }
    }
}