using Core.Interfaces.Repository;
using Core.Data.Entity;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repository
{
    public class VisitRepository : IVisitRepository
    {
        private readonly string _connectionString;

        public VisitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        }
        public async Task<List<int>> GetVisitedProfileIdsAsync(int userId)
        {
            var visited = new List<int>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT user_id FROM visits WHERE visited_user_id = @userId";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                visited.Add(reader.GetInt32(reader.GetOrdinal("visited_user_id")));
            }

            return visited;
        }
        public async Task<List<int>> GetProfileVisitorsIdsAsync(int userId)
        {
            var visitors = new List<int>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT user_id FROM visits WHERE visited_user_id = @userId";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                visitors.Add(reader.GetInt32(reader.GetOrdinal("visitor_user_id")));
            }

            return visitors;
        }
    }
}