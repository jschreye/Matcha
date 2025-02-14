using MySql.Data.MySqlClient;
using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly string _connectionString;

        public TagRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            var tags = new List<Tag>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT id, libelle FROM tags", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tags.Add(new Tag
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Libelle = reader.GetString(reader.GetOrdinal("libelle"))
                        });
                    }
                }
            }
            return tags;
        }

        public async Task SaveUserTagsAsync(int userId, List<int> tagIds)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var deleteCommand = new MySqlCommand("DELETE FROM userTags WHERE user_id = @userId", connection))
                {
                    deleteCommand.Parameters.AddWithValue("@userId", userId);
                    await deleteCommand.ExecuteNonQueryAsync();
                }

                foreach (var tagId in tagIds)
                {
                    using (var insertCommand = new MySqlCommand("INSERT INTO userTags (user_id, tag_id) VALUES (@userId, @tagId)", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@userId", userId);
                        insertCommand.Parameters.AddWithValue("@tagId", tagId);
                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }
}