using Core.Interfaces.Repository;
using Core.Data.Entity;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.Data.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(IConfiguration config, ILogger<MessageRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
            _logger = logger;
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
                    Id         = reader.GetInt32   (reader.GetOrdinal("id")),
                    SenderId   = reader.GetInt32   (reader.GetOrdinal("sender_id")),
                    ReceiverId = reader.GetInt32   (reader.GetOrdinal("receiver_id")),
                    Contenu    = reader.GetString  (reader.GetOrdinal("contenu")),
                    Timestamp  = reader.GetDateTime(reader.GetOrdinal("timestamp"))
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
                (SELECT contenu FROM messages 
                 WHERE (sender_id = u.id AND receiver_id = @userId) OR (sender_id = @userId AND receiver_id = u.id)
                 ORDER BY timestamp DESC LIMIT 1) AS last_message,
                (SELECT timestamp FROM messages 
                 WHERE (sender_id = u.id AND receiver_id = @userId) OR (sender_id = @userId AND receiver_id = u.id)
                 ORDER BY timestamp DESC LIMIT 1) AS last_message_date
            FROM users u
            INNER JOIN (
                SELECT 
                    CASE
                        WHEN sender_id = @userId THEN receiver_id
                        ELSE sender_id
                    END AS other_user_id
                FROM messages
                WHERE sender_id = @userId OR receiver_id = @userId
            ) conv ON conv.other_user_id = u.id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                conversations.Add(new ConversationDto
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    LastMessage = !reader.IsDBNull(reader.GetOrdinal("last_message")) 
                        ? reader.GetString(reader.GetOrdinal("last_message")) 
                        : null,
                    LastMessageDate = !reader.IsDBNull(reader.GetOrdinal("last_message_date")) 
                        ? reader.GetDateTime(reader.GetOrdinal("last_message_date"))
                        : DateTime.Now,
                    Photo = null // Sera rempli par le service photo
                });
            }
            return conversations;
        }

        public async Task<List<ConversationDto>> GetUnreadMessagesAsync(int userId)
        {
            try
            {
                var unreadMessages = new List<ConversationDto>();
                
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                
                // Version simplifiée qui ne récupère que les informations de base des notifications
                var query = @"
                    SELECT 
                        u.id AS user_id,
                        u.username,
                        MAX(n.timestamp) AS last_message_date,
                        COUNT(n.id) AS message_count
                    FROM notifications n
                    JOIN users u ON n.sender_id = u.id
                    WHERE n.user_id = @userId 
                        AND n.notification_type_id = 1 -- Type Message
                        AND n.lu = FALSE
                    GROUP BY n.sender_id, u.id, u.username
                    ORDER BY MAX(n.timestamp) DESC
                    LIMIT 5";
                
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var messageCount = reader.GetInt32(reader.GetOrdinal("message_count"));
                    
                    unreadMessages.Add(new ConversationDto
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                        Username = reader.GetString(reader.GetOrdinal("username")),
                        LastMessage = $"{messageCount} nouveau(x) message(s)",
                        LastMessageDate = reader.GetDateTime(reader.GetOrdinal("last_message_date")),
                        Photo = null // Sera rempli par le service photo
                    });
                }
                
                return unreadMessages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des messages non lus pour l'utilisateur {UserId}", userId);
                return new List<ConversationDto>();
            }
        }
    }
}