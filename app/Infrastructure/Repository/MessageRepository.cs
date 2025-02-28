using Core.Interfaces.Repository;
using Core.Data.Entity;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

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
    }
}