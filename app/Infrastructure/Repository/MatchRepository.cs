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

            // Toujours enregistrer user1 < user2 pour respecter la clé unique
            var orderedIds = new[] { userId1, userId2 }.OrderBy(id => id).ToArray();

            var query = @"
                INSERT IGNORE INTO matches (user1_id, user2_id)
                VALUES (@user1, @user2);";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@user1", orderedIds[0]);
            command.Parameters.AddWithValue("@user2", orderedIds[1]);

            await command.ExecuteNonQueryAsync();
        }
        public async Task DeleteMatchAsync(int userId1, int userId2)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // On ordonne les IDs pour correspondre à la contrainte UNIQUE
            var orderedIds = new[] { userId1, userId2 }.OrderBy(id => id).ToArray();

            var query = @"
                DELETE FROM matches
                WHERE user1_id = @user1 AND user2_id = @user2";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@user1", orderedIds[0]);
            command.Parameters.AddWithValue("@user2", orderedIds[1]);

            await command.ExecuteNonQueryAsync();
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