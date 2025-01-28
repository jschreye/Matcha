using MySql.Data.MySqlClient;
using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;
using Microsoft.Extensions.Configuration;
namespace Infrastructure.Repository
{
    public class PrefSexRepository : IPrefSexRepository
    {
        private readonly string _connectionString;

        public PrefSexRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        }
        public async Task<List<PrefSex>> GetPrefSexAsync()
        {
            var prefsexs = new List<PrefSex>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = "SELECT id, libelle FROM prefsex ORDER BY libelle";
                var command = new MySqlCommand(query, connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        prefsexs.Add(new PrefSex
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Libelle = reader.GetString(reader.GetOrdinal("libelle"))
                        });
                    }
                }
            }

            return prefsexs;
        }
    }
}