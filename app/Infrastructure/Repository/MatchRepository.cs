using Core.Interfaces.Repository;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Data.DTOs;
using Core.Data.Entity;

namespace Infrastructure.Repository
{
   public class MatchRepository : IMatchRepository
   {
        private readonly string _connectionString;
        private readonly ILogger<MatchRepository> _logger;

        public MatchRepository(IConfiguration config, ILogger<MatchRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
            _logger = logger;
        }
    public async Task<bool> CreateMatchAsync(int userId1, int userId2)
    {
        var ordered = new[] { userId1, userId2 }.OrderBy(i => i).ToArray();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // 1) Vérifier si une ligne existe déjà
        var checkSql = @"
        SELECT is_active
            FROM matches
        WHERE user1_id = @u1 AND user2_id = @u2
        LIMIT 1";
        using var checkCmd = new MySqlCommand(checkSql, connection);
        checkCmd.Parameters.AddWithValue("@u1", ordered[0]);
        checkCmd.Parameters.AddWithValue("@u2", ordered[1]);
        var exists = await checkCmd.ExecuteScalarAsync();
        if (exists != null)
        {
            bool wasActive = Convert.ToBoolean(exists);
            if (wasActive)
            {
                // match déjà actif : rien de neuf
                return false;
            }
            // match existait mais inactif : on le réactive
            var reactSql = @"
            UPDATE matches
                SET is_active = TRUE, matched_at = NOW()
            WHERE user1_id = @u1 AND user2_id = @u2";
            using var reactCmd = new MySqlCommand(reactSql, connection);
            reactCmd.Parameters.AddWithValue("@u1", ordered[0]);
            reactCmd.Parameters.AddWithValue("@u2", ordered[1]);
            await reactCmd.ExecuteNonQueryAsync();
            return true;
        }
        // 2) Pas de ligne : on insère un nouveau match
        var insertSql = @"
        INSERT INTO matches (user1_id, user2_id, matched_at, is_active)
            VALUES (@u1, @u2, NOW(), TRUE)";
        using var insCmd = new MySqlCommand(insertSql, connection);
        insCmd.Parameters.AddWithValue("@u1", ordered[0]);
        insCmd.Parameters.AddWithValue("@u2", ordered[1]);
        await insCmd.ExecuteNonQueryAsync();
        return true;
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

        public async Task<bool> HasMatchAsync(int userId1, int userId2)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var query = @"
                SELECT COUNT(*) FROM matches 
                WHERE 
                    ((user1_id = @UserId1 AND user2_id = @UserId2) OR (user1_id = @UserId2 AND user2_id = @UserId1))
                    AND is_active = TRUE";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId1", userId1);
            command.Parameters.AddWithValue("@UserId2", userId2);
            
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }
        
        public async Task<List<int>> GetUserMatchesAsync(int userId)
        {
            var result = new List<int>();
            
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var query = @"
                SELECT 
                    CASE 
                        WHEN user1_id = @UserId THEN user2_id 
                        ELSE user1_id 
                    END as matched_user_id
                FROM matches 
                WHERE 
                    (user1_id = @UserId OR user2_id = @UserId)
                    AND is_active = TRUE";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(reader.GetInt32(reader.GetOrdinal("matched_user_id")));
            }
            
            return result;
        }

        public async Task<List<UserDto>> GetRecentLikesAsync(int userId)
        {
            try
            {
                var likes = new List<UserDto>();
                
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var query = @"
                    SELECT u.id, u.username, u.email, u.age, u.created_at
                    FROM likes l
                    JOIN users u ON l.user_id = u.id
                    WHERE l.liked_user_id = @userId
                    ORDER BY l.timestamp DESC
                    LIMIT 5";
                
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    likes.Add(new UserDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Age = !reader.IsDBNull(reader.GetOrdinal("age")) ? reader.GetInt32(reader.GetOrdinal("age")) : 0,
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                        Photo = null // Sera rempli par le service photo
                    });
                }
                
                return likes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des likes récents pour l'utilisateur {UserId}", userId);
                return new List<UserDto>();
            }
        }
        
        public async Task<List<UserDto>> GetMatchesAsync(int userId)
        {
            try
            {
                var matches = new List<UserDto>();
                
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var query = @"
                    SELECT u.id, u.username, u.email, u.age, u.created_at
                    FROM matches m
                    JOIN users u ON 
                        CASE 
                            WHEN m.user1_id = @userId THEN m.user2_id = u.id
                            ELSE m.user1_id = u.id
                        END
                    WHERE (m.user1_id = @userId OR m.user2_id = @userId)
                        AND m.is_active = TRUE";
                
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    matches.Add(new UserDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        Email = reader.GetString(reader.GetOrdinal("email")),
                        Age = !reader.IsDBNull(reader.GetOrdinal("age")) ? reader.GetInt32(reader.GetOrdinal("age")) : 0,
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                        Photo = null // Sera rempli par le service photo
                    });
                }
                
                return matches;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des matchs pour l'utilisateur {UserId}", userId);
                return new List<UserDto>();
            }
        }
        
        public async Task<bool> AddMatchAsync(int user1Id, int user2Id)
        {
            try
            {
                var ordered = new[] { user1Id, user2Id }.OrderBy(i => i).ToArray();
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var query = @"
                    INSERT INTO matches (user1_id, user2_id, matched_at, is_active)
                    VALUES (@u1, @u2, NOW(), TRUE)
                    ON DUPLICATE KEY UPDATE 
                        is_active = TRUE,
                        matched_at = NOW()";
                
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@u1", ordered[0]);
                command.Parameters.AddWithValue("@u2", ordered[1]);
                
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout du match entre {User1Id} et {User2Id}", user1Id, user2Id);
                return false;
            }
        }
        
        public async Task<bool> AddLikeAsync(int userId, int likedUserId)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var query = @"
                    INSERT INTO likes (user_id, liked_user_id, timestamp)
                    VALUES (@userId, @likedUserId, NOW())
                    ON DUPLICATE KEY UPDATE
                        timestamp = NOW()";
                
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@likedUserId", likedUserId);
                
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout du like de {UserId} à {LikedUserId}", userId, likedUserId);
                return false;
            }
        }
        
        public async Task<bool> HasLikeAsync(int userId, int likedUserId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            var query = @"
                SELECT COUNT(*) FROM likes 
                WHERE user_id = @UserId AND liked_user_id = @LikedUserId";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@LikedUserId", likedUserId);
            
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }
    }
}