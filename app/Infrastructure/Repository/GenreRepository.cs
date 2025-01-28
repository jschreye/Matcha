using MySql.Data.MySqlClient;
using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly string _connectionString;

        public GenreRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        }
        public async Task<List<Genre>> GetGenresAsync()
        {
            var genres = new List<Genre>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "SELECT id, libelle FROM genre ORDER BY libelle";
                var command = new MySqlCommand(query, connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        genres.Add(new Genre
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Libelle = reader.GetString(reader.GetOrdinal("libelle"))
                        });
                    }
                }
            }

            return genres;
        }
    }
}