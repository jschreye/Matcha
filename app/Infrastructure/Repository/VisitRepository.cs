using Core.Interfaces.Repository;
using Core.Data.Entity;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class VisitRepository : IVisitRepository
    {
        private readonly string _connectionString;

        public VisitRepository(IConfiguration configuration)
        {
            _connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Chaîne 'DefaultConnection' introuvable.");
        }

        public async Task<List<int>> GetVisitedProfileIdsAsync(int userId)
        {
            var visited = new List<int>();
            const string sql = @"
                SELECT v.visited_user_id
                FROM visits AS v
                LEFT JOIN blocksReports AS b
                ON b.user_id         = v.visited_user_id
                AND b.blocked_user_id = @userId
                WHERE v.user_id          = @userId
                AND b.user_id IS NULL
                ORDER BY v.timestamp DESC;
            ";

            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                visited.Add(reader.GetInt32(reader.GetOrdinal("visited_user_id")));
            }
            return visited;
        }

        // 2) Les visiteurs du profil userId, sauf ceux que userId a bloqués
        public async Task<List<int>> GetProfileVisitorsIdsAsync(int userId)
        {
            var visitors = new List<int>();
            const string sql = @"
                SELECT v.user_id
                FROM visits AS v
                LEFT JOIN blocksReports AS b
                ON b.user_id         = v.user_id     
                AND b.blocked_user_id = @userId       
                WHERE v.visited_user_id = @userId        
                AND b.user_id IS NULL                
                ORDER BY v.timestamp DESC;
            ";

            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                visitors.Add(reader.GetInt32(reader.GetOrdinal("user_id")));
            }
            return visitors;
        }

        public async Task AddVisitAsync(int visitorId, int visitedId)
        {
            if (visitorId == visitedId) return;

            const string sql = @"
                INSERT INTO visits (user_id, visited_user_id, timestamp)
                VALUES (@visitor, @visited, NOW());
            ";

            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@visitor", visitorId);
            cmd.Parameters.AddWithValue("@visited", visitedId);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}