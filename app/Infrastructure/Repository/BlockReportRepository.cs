using Core.Interfaces.Repository;
using Core.Data.Entity;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Infrastructure.Repository
{
    public class BlockReportRepository : IBlockReportRepository
    {
        private readonly string _conn;
        public BlockReportRepository(IConfiguration cfg)
            => _conn = cfg.GetConnectionString("DefaultConnection")!;

        public async Task AddBlockAsync(int userId, int blockedUserId)
        {
            const string sql = @"
                INSERT IGNORE INTO blocksReports 
                (user_id, blocked_user_id, report_reason, timestamp) 
                VALUES (@user, @blocked, NULL, NOW());";
            await using var cn = new MySqlConnection(_conn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@user", userId);
            cmd.Parameters.AddWithValue("@blocked", blockedUserId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RemoveBlockAsync(int userId, int blockedUserId)
        {
            const string sql = @"
                DELETE FROM blocksReports
                WHERE user_id = @user AND blocked_user_id = @blocked;";
            await using var cn = new MySqlConnection(_conn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@user", userId);
            cmd.Parameters.AddWithValue("@blocked", blockedUserId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> IsBlockedAsync(int userId, int otherUserId)
        {
            const string sql = @"
                SELECT EXISTS(
                SELECT 1 
                    FROM blocksReports
                WHERE user_id = @user 
                    AND blocked_user_id = @other
                );";
            await using var cn = new MySqlConnection(_conn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@user", userId);
            cmd.Parameters.AddWithValue("@other", otherUserId);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync()) == 1;
        }

        public async Task AddReportAsync(int userId, int reportedUserId, string reason)
        {
            const string sql = @"
                INSERT INTO blocksReports 
                (user_id, blocked_user_id, report_reason, timestamp)
                VALUES (@user, @reported, @reason, NOW())
                ON DUPLICATE KEY UPDATE report_reason = @reason, timestamp = NOW();";
            await using var cn = new MySqlConnection(_conn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@user", userId);
            cmd.Parameters.AddWithValue("@reported", reportedUserId);
            cmd.Parameters.AddWithValue("@reason", reason);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<(int ReportedUserId, string Reason, DateTime Timestamp)>> GetReportsByUserAsync(int userId)
        {
            var list = new List<(int, string, DateTime)>();
            const string sql = @"
                SELECT blocked_user_id, report_reason, timestamp
                FROM blocksReports
                WHERE user_id = @user
            ORDER BY timestamp DESC;";
            await using var cn = new MySqlConnection(_conn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@user", userId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add((
                reader.GetInt32(reader.GetOrdinal("blocked_user_id")),
                reader.GetString(reader.GetOrdinal("report_reason")),
                reader.GetDateTime(reader.GetOrdinal("timestamp"))
                ));
            }
            return list;
        }
        public async Task<bool> HasBlockBetweenAsync(int userId1, int userId2)
        {
            const string sql = @"
                SELECT EXISTS(
                    SELECT 1
                    FROM blocksReports
                    WHERE (user_id = @u1  
                        AND blocked_user_id = @u2)
                    OR (user_id = @u2  
                        AND blocked_user_id = @u1)
                )";
            await using var cn = new MySqlConnection(_conn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@u1", userId1);
            cmd.Parameters.AddWithValue("@u2", userId2);
            var exists = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(exists) == 1;
        }
    }
}