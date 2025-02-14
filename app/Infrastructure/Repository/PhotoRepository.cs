using Core.Interfaces.Repository;
using Core.Data.Entity;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly string _connectionString;

        public PhotoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
        }

        public async Task InsertPhotoAsync(Photo photo)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO photos (user_id, image_data, est_profil)
                VALUES (@UserId, @ImageData, @EstProfil);
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", photo.UserId);
            cmd.Parameters.AddWithValue("@ImageData", photo.ImageData);
            cmd.Parameters.Add("@EstProfil", MySqlDbType.Bit).Value = photo.EstProfil ? 1 : 0;

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task AddPhotoWithProfileAsync(Photo photo)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Supprimer l'ancienne photo de profil si elle existe
                var oldProfilePhoto = await GetProfilePhotoAsync(photo.UserId);
                if (oldProfilePhoto != null)
                {
                    await DeletePhotoAsync(oldProfilePhoto.Id);
                }

                // Insérer la nouvelle photo comme photo de profil
                var insertQuery = @"
                    INSERT INTO photos (user_id, image_data, est_profil)
                    VALUES (@UserId, @ImageData, @EstProfil);
                ";

                using var insertCmd = new MySqlCommand(insertQuery, connection, (MySqlTransaction)transaction);
                insertCmd.Parameters.AddWithValue("@UserId", photo.UserId);
                insertCmd.Parameters.AddWithValue("@ImageData", photo.ImageData);
                insertCmd.Parameters.Add("@EstProfil", MySqlDbType.Bit).Value = photo.EstProfil ? 1 : 0;

                await insertCmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Photo?> GetProfilePhotoAsync(int userId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT id, user_id, image_data, est_profil
                FROM photos
                WHERE user_id = @UserId AND est_profil = TRUE
                LIMIT 1;
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // Obtenir les indices des colonnes
                int idIndex = reader.GetOrdinal("id");
                int userIdIndex = reader.GetOrdinal("user_id");
                int estProfilIndex = reader.GetOrdinal("est_profil");

                return new Photo
                {
                    Id = reader.GetInt32(idIndex),
                    UserId = reader.GetInt32(userIdIndex),
                    ImageData = (byte[])reader["image_data"], // Correct comme tu l’as déjà
                    EstProfil = reader.GetBoolean(estProfilIndex)
                };
            }

            return null;
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                DELETE FROM photos
                WHERE id = @PhotoId;
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@PhotoId", photoId);

            await cmd.ExecuteNonQueryAsync();
        }
        public async Task<List<Photo>> GetUserPhotosAsync(int userId)
        {
            var photos = new List<Photo>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT id, user_id, image_data, est_profil
                FROM photos
                WHERE user_id = @UserId AND est_profil = FALSE;
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // Obtenir les indices des colonnes
                int idIndex = reader.GetOrdinal("id");
                int userIdIndex = reader.GetOrdinal("user_id");
                int imageDataIndex = reader.GetOrdinal("image_data");
                int estProfilIndex = reader.GetOrdinal("est_profil");

                photos.Add(new Photo
                {
                    Id = reader.GetInt32(idIndex),
                    UserId = reader.GetInt32(userIdIndex),
                    ImageData = (byte[])reader["image_data"], // Utilisation de l'indexeur pour les données binaires
                    EstProfil = reader.GetBoolean(estProfilIndex)
                });
            }

            return photos;
        }

        public async Task UpdateProfilePhotoAsync(int userId, int newProfilePhotoId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Désactiver l'ancienne photo de profil
                var updateOldQuery = @"
                    UPDATE photos
                    SET est_profil = 0
                    WHERE user_id = @UserId AND est_profil = 1;
                ";
                using var updateOldCmd = new MySqlCommand(updateOldQuery, connection, (MySqlTransaction)transaction);
                updateOldCmd.Parameters.AddWithValue("@UserId", userId);
                await updateOldCmd.ExecuteNonQueryAsync();

                // Activer la nouvelle photo de profil
                var updateNewQuery = @"
                    UPDATE photos
                    SET est_profil = 1
                    WHERE id = @NewProfilePhotoId;
                ";
                using var updateNewCmd = new MySqlCommand(updateNewQuery, connection, (MySqlTransaction)transaction);
                updateNewCmd.Parameters.AddWithValue("@NewProfilePhotoId", newProfilePhotoId);
                await updateNewCmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}