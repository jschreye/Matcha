using Core.Interfaces.Repository;
using Core.Data.Entity;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.Data.DTOs;

namespace Infrastructure.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;

        public MessageRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
        }

        public async Task SaveMessageAsync(Message message)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new MySqlCommand(
                "INSERT INTO messages (sender_id, receiver_id, contenu, timestamp) VALUES (@senderId, @receiverId, @contenu, @timestamp)",
                connection);
            command.Parameters.AddWithValue("@senderId", message.SenderId);
            command.Parameters.AddWithValue("@receiverId", message.ReceiverId);
            command.Parameters.AddWithValue("@contenu", message.Contenu);
            command.Parameters.AddWithValue("@timestamp", message.Timestamp);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Message>> GetConversationAsync(int userId1, int userId2)
        {
            var messages = new List<Message>();
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var command = new MySqlCommand(
                @"SELECT id, sender_id, receiver_id, contenu, timestamp 
                FROM messages 
                WHERE (sender_id = @user1 AND receiver_id = @user2) 
                    OR (sender_id = @user2 AND receiver_id = @user1)
                ORDER BY timestamp", 
                connection);
            command.Parameters.AddWithValue("@user1", userId1);
            command.Parameters.AddWithValue("@user2", userId2);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new Message
                {
                    Id = reader.GetOrdinal("id"),
                    SenderId = reader.GetOrdinal("sender_id"),
                    ReceiverId = reader.GetOrdinal("receiver_id"),
                    Contenu = reader.GetString(reader.GetOrdinal("contenu")),
                    Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp"))
                });
            }
            return messages;
        }

        public async Task<List<ConversationDto>> GetConversationsForUserAsync(int userId)
        {
            var conversations = new List<ConversationDto>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT DISTINCT
                u.id AS user_id,
                u.username,
                p.image_data
            FROM users u
            INNER JOIN (
                SELECT 
                    CASE
                        WHEN sender_id = @userId THEN receiver_id
                        ELSE sender_id
                    END AS other_user_id
                FROM messages
                WHERE sender_id = @userId OR receiver_id = @userId
            ) conv ON conv.other_user_id = u.id
            LEFT JOIN photos p ON p.user_id = u.id AND p.est_profil = TRUE";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var photoBytes = reader["image_data"] as byte[];
                string? photoBase64 = photoBytes != null
                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(photoBytes)}"
                    : null;

                conversations.Add(new ConversationDto
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Photo = photoBase64
                });
            }
            return conversations;
        }
    }
}