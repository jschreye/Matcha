using MySql.Data.MySqlClient;
using Core.Data.Entity;
using Core.Data.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Core.Interfaces.Repository;
public class NotificationRepository : INotificationRepository
{
    private readonly string _connectionString;

    public NotificationRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    }

    public async Task SaveNotificationAsync(Notification notification)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            "INSERT INTO notifications (user_id, sender_id, notification_type_id, lu, timestamp) VALUES (@userId, @senderId, @notificationTypeId, @lu, @timestamp)",
            connection);

        command.Parameters.AddWithValue("@userId", notification.UserId);
        command.Parameters.AddWithValue("@senderId", notification.SenderId);
        // Si NotificationTypeId est nullable, on gère le cas où il n'est pas renseigné
        command.Parameters.AddWithValue("@notificationTypeId", notification.NotificationTypeId.HasValue 
            ? (object)notification.NotificationTypeId.Value 
            : DBNull.Value);
        command.Parameters.AddWithValue("@lu", notification.Lu);
        command.Parameters.AddWithValue("@timestamp", notification.Timestamp);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<NotificationDto>> GetNotificationsForUserAsync(int userId)
    {
        var notifications = new List<NotificationDto>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new MySqlCommand(
            @"SELECT n.id, n.user_id, 
                     u.username AS sender_username, 
                     nt.libelle AS type_libelle, 
                     n.lu, n.timestamp 
              FROM notifications n
              LEFT JOIN notification_types nt ON n.notification_type_id = nt.id
              LEFT JOIN users u ON n.sender_id = u.id
              WHERE n.user_id = @userId
              ORDER BY n.timestamp DESC",
            connection);

        command.Parameters.AddWithValue("@userId", userId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            notifications.Add(new NotificationDto
            {
                Id = reader.GetOrdinal("id"),
                UserId = reader.GetOrdinal("user_id"),
                SenderUsername = reader.IsDBNull(reader.GetOrdinal("sender_username"))
                    ? "Utilisateur inconnu"
                    : reader.GetString(reader.GetOrdinal("sender_username")),
                TypeLibelle = reader.IsDBNull(reader.GetOrdinal("type_libelle"))
                    ? "Autres"
                    : reader.GetString(reader.GetOrdinal("type_libelle")),
                Lu = reader.GetBoolean(reader.GetOrdinal("lu")),
            });
        }

        return notifications;
    }
    public async Task DeleteNotificationAsync(int notificationId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new MySqlCommand(
            "DELETE FROM notifications WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", notificationId);
        await command.ExecuteNonQueryAsync();
    }
}
