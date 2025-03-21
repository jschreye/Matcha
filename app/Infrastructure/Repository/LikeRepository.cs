using Core.Interfaces.Repository;
using Core.Data.Entity;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
   public class LikeRepository : ILikeRepository
   {

      private readonly string _connectionString;
      public LikeRepository(IConfiguration config)
      {
         _connectionString = config.GetConnectionString("DefaultConnection")
               ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
      }

      public async Task LikeProfileAsync(int userId, int likedUserId)
      {
         using var connection = new MySqlConnection(_connectionString);
         await connection.OpenAsync();

         // Vérifier si l'utilisateur a déjà liké ce profil
         var checkQuery = "SELECT COUNT(*) FROM likes WHERE user_id = @userId AND liked_user_id = @likedUserId";
         using var checkCommand = new MySqlCommand(checkQuery, connection);
         checkCommand.Parameters.AddWithValue("@userId", userId);
         checkCommand.Parameters.AddWithValue("@likedUserId", likedUserId);

         var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

         if (count == 0)
         {
            // Si l'utilisateur n'a pas encore liké ce profil, ajouter une entrée dans la table
            var insertQuery = "INSERT INTO likes (user_id, liked_user_id) VALUES (@userId, @likedUserId)";
            using var insertCommand = new MySqlCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@userId", userId);
            insertCommand.Parameters.AddWithValue("@likedUserId", likedUserId);
            await insertCommand.ExecuteNonQueryAsync();
         }
      }
      public async Task UnlikeProfileAsync(int userId, int likedUserId)
      {
         using var connection = new MySqlConnection(_connectionString);
         await connection.OpenAsync();

         // Supprimer l'entrée dans la table "likes"
         var deleteQuery = "DELETE FROM likes WHERE user_id = @userId AND liked_user_id = @likedUserId";
         using var deleteCommand = new MySqlCommand(deleteQuery, connection);
         deleteCommand.Parameters.AddWithValue("@userId", userId);
         deleteCommand.Parameters.AddWithValue("@likedUserId", likedUserId);
         await deleteCommand.ExecuteNonQueryAsync();
      }
   }
}