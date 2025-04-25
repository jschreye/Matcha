using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Core.Data.DTOs;
using Core.Data.Entity;
using Core.Interfaces.Repository;

namespace Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        }

        public async Task<List<UserDto>> GetAllUserAsync()
        {
            var users = new List<UserDto>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT id, username, email, age, created_at FROM users", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new UserDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.GetString(reader.GetOrdinal("email")),
                    Age = reader.GetInt32(reader.GetOrdinal("age")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                });
            }

            return users;
        }

        public async Task<User?> GetByEmail(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand(@"
                SELECT 
                    id, firstname, lastname, username, email,
                    password_hash, isactive, activationtoken, passwordresettoken, created_at 
                FROM users 
                WHERE email = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Firstname = reader.IsDBNull(reader.GetOrdinal("firstname")) ? string.Empty : reader.GetString(reader.GetOrdinal("firstname")),
                    Lastname = reader.IsDBNull(reader.GetOrdinal("lastname")) ? string.Empty : reader.GetString(reader.GetOrdinal("lastname")),
                    Username = reader.IsDBNull(reader.GetOrdinal("username")) ? string.Empty : reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                    PasswordHash = reader.IsDBNull(reader.GetOrdinal("password_hash")) ? string.Empty : reader.GetString(reader.GetOrdinal("password_hash")),
                    IsActive = !reader.IsDBNull(reader.GetOrdinal("isactive")) && reader.GetBoolean(reader.GetOrdinal("isactive")),
                    ActivationToken = reader.IsDBNull(reader.GetOrdinal("activationtoken"))
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("activationtoken")),
                    PasswordResetToken = reader.IsDBNull(reader.GetOrdinal("passwordresettoken"))
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("passwordresettoken")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }
            return null;
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand(@"
                SELECT 
                    id, firstname, lastname, username, email,
                    password_hash, isactive, activationtoken, profile_complete, 
                    localisation_isactive, created_at 
                FROM users 
                WHERE username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Firstname = reader.IsDBNull(reader.GetOrdinal("firstname")) ? string.Empty : reader.GetString(reader.GetOrdinal("firstname")),
                    Lastname = reader.IsDBNull(reader.GetOrdinal("lastname")) ? string.Empty : reader.GetString(reader.GetOrdinal("lastname")),
                    Username = reader.IsDBNull(reader.GetOrdinal("username")) ? string.Empty : reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                    PasswordHash = reader.IsDBNull(reader.GetOrdinal("password_hash")) ? string.Empty : reader.GetString(reader.GetOrdinal("password_hash")),
                    IsActive = !reader.IsDBNull(reader.GetOrdinal("isactive"))
                                    && reader.GetBoolean(reader.GetOrdinal("isactive")),
                    ActivationToken = reader.IsDBNull(reader.GetOrdinal("activationtoken")) 
                                    ? null 
                                    : reader.GetString(reader.GetOrdinal("activationtoken")),
                    ProfileComplete = !reader.IsDBNull(reader.GetOrdinal("profile_complete"))
                                    && reader.GetBoolean(reader.GetOrdinal("profile_complete")),
                    LocalisationIsActive = !reader.IsDBNull(reader.GetOrdinal("localisation_isactive"))
                                    && reader.GetBoolean(reader.GetOrdinal("localisation_isactive")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            }
            return null;
        }

        public async Task Add(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO users 
                    (firstname, lastname, username, email, password_hash, isactive, activationtoken, created_at) 
                VALUES 
                    (@Firstname, @Lastname, @Username, @Email, @PasswordHash, @IsActive, @ActivationToken, @CreatedAt)";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Firstname", user.Firstname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Lastname", user.Lastname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Username", user.Username ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@ActivationToken", user.ActivationToken ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE users 
                SET 
                    firstname = @Firstname,
                    lastname = @Lastname,
                    username = @Username,
                    email = @Email,
                    age = @Age,
                    password_hash = @PasswordHash,
                    isactive = @IsActive,
                    activationtoken = @ActivationToken,
                    passwordresettoken = @PasswordResetToken
                WHERE id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Firstname", user.Firstname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Lastname", user.Lastname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Username", user.Username ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Age", user.Age);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@ActivationToken", user.ActivationToken ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordResetToken", user.PasswordResetToken ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Id", user.Id);

            await command.ExecuteNonQueryAsync();
        }
        public async Task<string?> GetPasswordHashByUsernameAsync(string username)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT password_hash FROM users WHERE username=@username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);

            var dbPasswordHash = await command.ExecuteScalarAsync() as string;
            return dbPasswordHash;
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand(@"
                SELECT 
                    id, firstname, lastname, username, email, age,
                    password_hash, isactive, activationtoken, passwordresettoken, 
                    genre_id, tag_id, sexual_preferences_id, biography, 
                    ST_AsText(gps_location) as gps_location,
                    popularity_score, notifisactive, profile_complete, localisation_isactive,
                    created_at 
                FROM users 
                WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var user = new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Firstname = reader.IsDBNull(reader.GetOrdinal("firstname")) ? string.Empty : reader.GetString(reader.GetOrdinal("firstname")),
                    Lastname = reader.IsDBNull(reader.GetOrdinal("lastname")) ? string.Empty : reader.GetString(reader.GetOrdinal("lastname")),
                    Username = reader.IsDBNull(reader.GetOrdinal("username")) ? string.Empty : reader.GetString(reader.GetOrdinal("username")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                    Age = reader.GetInt32(reader.GetOrdinal("age")),
                    PasswordHash = reader.IsDBNull(reader.GetOrdinal("password_hash")) ? string.Empty : reader.GetString(reader.GetOrdinal("password_hash")),
                    IsActive = !reader.IsDBNull(reader.GetOrdinal("isactive")) && reader.GetBoolean(reader.GetOrdinal("isactive")),
                    ActivationToken = reader.IsDBNull(reader.GetOrdinal("activationtoken")) ? null : reader.GetString(reader.GetOrdinal("activationtoken")),
                    PasswordResetToken = reader.IsDBNull(reader.GetOrdinal("passwordresettoken")) ? null : reader.GetString(reader.GetOrdinal("passwordresettoken")),
                    Genre = reader.IsDBNull(reader.GetOrdinal("genre_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("genre_id")),
                    Tag = reader.IsDBNull(reader.GetOrdinal("tag_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("tag_id")),
                    SexualPreferences = reader.IsDBNull(reader.GetOrdinal("sexual_preferences_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("sexual_preferences_id")),
                    Biography = reader.IsDBNull(reader.GetOrdinal("biography")) ? null : reader.GetString(reader.GetOrdinal("biography")),
                    Latitude = reader.IsDBNull(reader.GetOrdinal("gps_location")) ? (double?)null : ParseLatitude(reader.GetString(reader.GetOrdinal("gps_location"))),
                    Longitude = reader.IsDBNull(reader.GetOrdinal("gps_location")) ? (double?)null : ParseLongitude(reader.GetString(reader.GetOrdinal("gps_location"))),
                    PopularityScore = reader.GetInt32(reader.GetOrdinal("popularity_score")),
                    NotifIsActive = !reader.IsDBNull(reader.GetOrdinal("notifisactive")) && reader.GetBoolean(reader.GetOrdinal("notifisactive")),
                    ProfileComplete = !reader.IsDBNull(reader.GetOrdinal("profile_complete")) && reader.GetBoolean(reader.GetOrdinal("profile_complete")),
                    LocalisationIsActive = !reader.IsDBNull(reader.GetOrdinal("localisation_isactive")) && reader.GetBoolean(reader.GetOrdinal("localisation_isactive")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
                return user;
            }
            return null;
        }

        private double ParseLatitude(string point)
        {
            var parts = point.Replace("POINT(", "").Replace(")", "").Split(' ');
            if (parts.Length == 2 && double.TryParse(parts[1], out var lat))
                return lat;
            return 0.0;
        }

        private double ParseLongitude(string point)
        {
            var parts = point.Replace("POINT(", "").Replace(")", "").Split(' ');
            if (parts.Length == 2 && double.TryParse(parts[0], out var lon))
                return lon;
            return 0.0;
        }

        public async Task UpdateUserAsync(User user)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE users 
                SET 
                    firstname = @Firstname,
                    lastname = @Lastname,
                    username = @Username,
                    email = @Email,
                    age = @Age,
                    genre_id = @Genre,
                    tag_id = @Tag,
                    sexual_preferences_id = @SexualPreferences,
                    biography = @Biography,
                    gps_location = ST_GeomFromText(@GpsLocation),
                    profile_complete = @ProfileComplete,
                    notifisactive = @NotifIsActive,
                    localisation_isactive = @LocalisationIsActive
                WHERE id = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Firstname", user.Firstname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Lastname", user.Lastname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Username", user.Username ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Age", user.Age);
            command.Parameters.AddWithValue("@Genre", user.Genre ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Tag", user.Tag ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SexualPreferences", user.SexualPreferences ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Biography", user.Biography ?? (object)DBNull.Value);
            if (user.Latitude.HasValue && user.Longitude.HasValue)
            {
                command.Parameters.AddWithValue("@GpsLocation", $"POINT({user.Longitude.Value} {user.Latitude.Value})");
            }
            else
            {
                command.Parameters.AddWithValue("@GpsLocation", DBNull.Value);
            }
            command.Parameters.AddWithValue("@ProfileComplete", user.ProfileComplete);
            command.Parameters.AddWithValue("@LocalisationIsActive", user.LocalisationIsActive);
            command.Parameters.AddWithValue("@NotifIsActive", user.NotifIsActive);
            command.Parameters.AddWithValue("@Id", user.Id);

            await command.ExecuteNonQueryAsync();
        }
        public async Task<List<UserDto>> GetUsersByIdsAsync(List<int> userIds)
        {
            var users = new List<UserDto>();

            if (userIds == null || userIds.Count == 0)
                return users;

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var inClause = string.Join(",", userIds.Select((_, i) => $"@id{i}"));
            var query = $@"
                SELECT 
                    u.id, 
                    u.username, 
                    p.image_data
                FROM users u
                LEFT JOIN photos p ON p.user_id = u.id AND p.est_profil = TRUE
                WHERE u.id IN ({inClause})";

            using var command = new MySqlCommand(query, connection);
            for (int i = 0; i < userIds.Count; i++)
            {
                command.Parameters.AddWithValue($"@id{i}", userIds[i]);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                byte[]? photoData = reader["image_data"] as byte[];
                string? photoBase64 = photoData != null && photoData.Length > 0
                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(photoData)}"
                    : string.Empty;

                users.Add(new UserDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Photo = photoBase64
                });
            }
            return users;
        }
    }
}
