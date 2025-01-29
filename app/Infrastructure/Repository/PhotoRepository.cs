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
            cmd.Parameters.AddWithValue("@EstProfil", photo.EstProfil);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}