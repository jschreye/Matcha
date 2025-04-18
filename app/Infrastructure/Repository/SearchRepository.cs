using Core.Interfaces.Repository;
using System.Text;
using System.Data.Common;
using Core.Data.DTOs;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Infrastructure.Repository
{
    public class SearchRepository : ISearchRepository
    {
        private readonly string _connectionString;

        public SearchRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
        }

        public async Task<List<UserProfileDto>> GetSimilarProfilesAsync(int userId)
        {
            var userProfiles = new List<UserProfileDto>();
            
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // Récupérer d'abord les préférences et caractéristiques de l'utilisateur actuel
            int? userGenre = null;
            int? userSexualPreferences = null;
            int? userTag = null;
            double? userLatitude = null;
            double? userLongitude = null;

            var userQuery = @"
                SELECT 
                    genre_id, sexual_preferences_id, tag_id,
                    ST_X(gps_location) AS longitude, ST_Y(gps_location) AS latitude
                FROM users
                WHERE id = @UserId";

            using (var userCommand = new MySqlCommand(userQuery, connection))
            {
                userCommand.Parameters.AddWithValue("@UserId", userId);
                using var userReader = await userCommand.ExecuteReaderAsync();
                
                if (await userReader.ReadAsync())
                {
                    userGenre = !userReader.IsDBNull(userReader.GetOrdinal("genre_id")) 
                        ? userReader.GetInt32(userReader.GetOrdinal("genre_id")) 
                        : null;
                        
                    userSexualPreferences = !userReader.IsDBNull(userReader.GetOrdinal("sexual_preferences_id"))
                        ? userReader.GetInt32(userReader.GetOrdinal("sexual_preferences_id"))
                        : null;
                        
                    userTag = !userReader.IsDBNull(userReader.GetOrdinal("tag_id"))
                        ? userReader.GetInt32(userReader.GetOrdinal("tag_id"))
                        : null;
                    
                    if (!userReader.IsDBNull(userReader.GetOrdinal("longitude")) && 
                        !userReader.IsDBNull(userReader.GetOrdinal("latitude")))
                    {
                        userLongitude = userReader.GetDouble(userReader.GetOrdinal("longitude"));
                        userLatitude = userReader.GetDouble(userReader.GetOrdinal("latitude"));
                    }
                }
            }

            // Construire la requête en fonction des préférences sexuelles
            var queryBuilder = new StringBuilder(@"
                SELECT 
                    u.id, u.username, u.firstname, u.lastname, u.email, u.age,
                    u.biography, u.genre_id, u.tag_id, u.sexual_preferences_id,
                    ST_X(u.gps_location) AS longitude, ST_Y(u.gps_location) AS latitude,
                    u.popularity_score, u.profile_complete, u.isactive,
                    u.localisation_isactive, u.notifisactive, u.created_at,
                    (
                        -- Score basé sur les mêmes tags
                        CASE WHEN u.tag_id = @UserTag THEN 20 ELSE 0 END +
                        
                        -- Score basé sur l'âge (plus ils sont proches, plus le score est élevé)
                        CASE 
                            WHEN ABS(u.age - @UserAge) <= 5 THEN 10
                            WHEN ABS(u.age - @UserAge) <= 10 THEN 5
                            ELSE 0
                        END +
                        
                        -- Score basé sur la proximité géographique
                        CASE 
                            WHEN @UserLatitude IS NOT NULL AND @UserLongitude IS NOT NULL AND u.gps_location IS NOT NULL THEN
                                CASE
                                    WHEN ST_Distance_Sphere(
                                        POINT(@UserLongitude, @UserLatitude),
                                        u.gps_location
                                    ) <= 5000 THEN 15  -- 5 km
                                    WHEN ST_Distance_Sphere(
                                        POINT(@UserLongitude, @UserLatitude),
                                        u.gps_location
                                    ) <= 20000 THEN 10  -- 20 km
                                    WHEN ST_Distance_Sphere(
                                        POINT(@UserLongitude, @UserLatitude),
                                        u.gps_location
                                    ) <= 50000 THEN 5   -- 50 km
                                    ELSE 0
                                END
                            ELSE 0
                        END
                    ) AS similarity_score
                FROM 
                    users u
                WHERE 
                    u.id != @UserId AND
                    u.profile_complete = 1 AND
                    u.isactive = 1");

            // Ajout des conditions de filtrage selon les préférences sexuelles
            if (userGenre.HasValue && userSexualPreferences.HasValue)
            {
                // 1 = Homme, 2 = Femme, 3 = Autre dans la table genre
                // 1 = Hetero, 2 = Gay, 3 = Bisexual dans la table prefsex
                if (userGenre == 1) // Homme
                {
                    if (userSexualPreferences == 1) // Hétéro
                    {
                        // Montre uniquement les femmes hétéro
                        queryBuilder.Append(" AND u.genre_id = 2 AND u.sexual_preferences_id = 1");
                    }
                    else if (userSexualPreferences == 2) // Gay
                    {
                        // Montre uniquement les hommes gay
                        queryBuilder.Append(" AND u.genre_id = 1 AND u.sexual_preferences_id = 2");
                    }
                    else if (userSexualPreferences == 3) // Bisexuel
                    {
                        // Pas de filtrage par genre car bisexuel
                    }
                }
                else if (userGenre == 2) // Femme
                {
                    if (userSexualPreferences == 1) // Hétéro
                    {
                        // Montre uniquement les hommes hétéro
                        queryBuilder.Append(" AND u.genre_id = 1 AND u.sexual_preferences_id = 1");
                    }
                    else if (userSexualPreferences == 2) // Gay
                    {
                        // Montre uniquement les femmes gay
                        queryBuilder.Append(" AND u.genre_id = 2 AND u.sexual_preferences_id = 2");
                    }
                    else if (userSexualPreferences == 3) // Bisexuel
                    {
                        // Pas de filtrage par genre car bisexuel
                    }
                }
                // Pour le genre "Autre" (3), pas de filtrage spécifique
            }

            queryBuilder.Append(" ORDER BY similarity_score DESC LIMIT 10");

            using (var command = new MySqlCommand(queryBuilder.ToString(), connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                
                var userAge = await GetUserAgeAsync(userId);
                command.Parameters.AddWithValue("@UserAge", userAge);
                
                command.Parameters.AddWithValue("@UserGenre", userGenre ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserSexualPreferences", userSexualPreferences ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserTag", userTag ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserLatitude", userLatitude ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserLongitude", userLongitude ?? (object)DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    userProfiles.Add(MapToUserProfileDto(reader));
                }
            }

            return userProfiles;
        }

        public async Task<List<UserProfileDto>> GetRandomProfilesAsync(int userId, int count)
        {
            var userProfiles = new List<UserProfileDto>();
            
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // Récupérer d'abord les préférences et caractéristiques de l'utilisateur actuel
            int? userGenre = null;
            int? userSexualPreferences = null;

            var userQuery = @"
                SELECT 
                    genre_id, sexual_preferences_id
                FROM users
                WHERE id = @UserId";

            using (var userCommand = new MySqlCommand(userQuery, connection))
            {
                userCommand.Parameters.AddWithValue("@UserId", userId);
                using var userReader = await userCommand.ExecuteReaderAsync();
                
                if (await userReader.ReadAsync())
                {
                    userGenre = !userReader.IsDBNull(userReader.GetOrdinal("genre_id")) 
                        ? userReader.GetInt32(userReader.GetOrdinal("genre_id")) 
                        : null;
                        
                    userSexualPreferences = !userReader.IsDBNull(userReader.GetOrdinal("sexual_preferences_id"))
                        ? userReader.GetInt32(userReader.GetOrdinal("sexual_preferences_id"))
                        : null;
                }
            }

            // Construire la requête en fonction des préférences sexuelles
            var queryBuilder = new StringBuilder(@"
                SELECT 
                    u.id, u.username, u.firstname, u.lastname, u.email, u.age,
                    u.biography, u.genre_id, u.tag_id, u.sexual_preferences_id,
                    ST_X(u.gps_location) AS longitude, ST_Y(u.gps_location) AS latitude,
                    u.popularity_score, u.profile_complete, u.isactive,
                    u.localisation_isactive, u.notifisactive, u.created_at
                FROM 
                    users u
                WHERE 
                    u.id != @UserId AND
                    u.profile_complete = 1 AND
                    u.isactive = 1");

            // Ajout des conditions de filtrage selon les préférences sexuelles
            if (userGenre.HasValue && userSexualPreferences.HasValue)
            {
                // 1 = Homme, 2 = Femme, 3 = Autre dans la table genre
                // 1 = Hetero, 2 = Gay, 3 = Bisexual dans la table prefsex
                if (userGenre == 1) // Homme
                {
                    if (userSexualPreferences == 1) // Hétéro
                    {
                        // Montre uniquement les femmes hétéro
                        queryBuilder.Append(" AND u.genre_id = 2 AND u.sexual_preferences_id = 1");
                    }
                    else if (userSexualPreferences == 2) // Gay
                    {
                        // Montre uniquement les hommes gay
                        queryBuilder.Append(" AND u.genre_id = 1 AND u.sexual_preferences_id = 2");
                    }
                    else if (userSexualPreferences == 3) // Bisexuel
                    {
                        // Pas de filtrage par genre car bisexuel
                    }
                }
                else if (userGenre == 2) // Femme
                {
                    if (userSexualPreferences == 1) // Hétéro
                    {
                        // Montre uniquement les hommes hétéro
                        queryBuilder.Append(" AND u.genre_id = 1 AND u.sexual_preferences_id = 1");
                    }
                    else if (userSexualPreferences == 2) // Gay
                    {
                        // Montre uniquement les femmes gay
                        queryBuilder.Append(" AND u.genre_id = 2 AND u.sexual_preferences_id = 2");
                    }
                    else if (userSexualPreferences == 3) // Bisexuel
                    {
                        // Pas de filtrage par genre car bisexuel
                    }
                }
                // Pour le genre "Autre" (3), pas de filtrage spécifique
            }

            queryBuilder.Append(" ORDER BY RAND() LIMIT @Count");

            using var command = new MySqlCommand(queryBuilder.ToString(), connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Count", count);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                userProfiles.Add(MapToUserProfileDto(reader));
            }

            return userProfiles;
        }

        public async Task<List<UserProfileDto>> GetFilteredProfilesAsync(
            int userId,
            int? minAge,
            int? maxAge,
            int? minPopularity,
            int? maxPopularity,
            double? maxDistance,
            List<int>? tagIds)
        {
            var userProfiles = new List<UserProfileDto>();
            
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            // Récupérer les préférences de l'utilisateur actuel pour filtrer par genre/préférences sexuelles
            int? userGenre = null;
            int? userSexualPreferences = null;
            double? userLatitude = null;
            double? userLongitude = null;

            var userQuery = @"
                SELECT 
                    genre_id, sexual_preferences_id,
                    ST_X(gps_location) AS longitude, ST_Y(gps_location) AS latitude
                FROM users
                WHERE id = @UserId";

            using (var userCommand = new MySqlCommand(userQuery, connection))
            {
                userCommand.Parameters.AddWithValue("@UserId", userId);
                using var userReader = await userCommand.ExecuteReaderAsync();
                
                if (await userReader.ReadAsync())
                {
                    userGenre = !userReader.IsDBNull(userReader.GetOrdinal("genre_id")) 
                        ? userReader.GetInt32(userReader.GetOrdinal("genre_id")) 
                        : null;
                        
                    userSexualPreferences = !userReader.IsDBNull(userReader.GetOrdinal("sexual_preferences_id"))
                        ? userReader.GetInt32(userReader.GetOrdinal("sexual_preferences_id"))
                        : null;
                    
                    if (!userReader.IsDBNull(userReader.GetOrdinal("longitude")) && 
                        !userReader.IsDBNull(userReader.GetOrdinal("latitude")))
                    {
                        userLongitude = userReader.GetDouble(userReader.GetOrdinal("longitude"));
                        userLatitude = userReader.GetDouble(userReader.GetOrdinal("latitude"));
                    }
                }
            }

            // Vérifier si l'utilisateur a des préférences sexuelles définies
            if (!userGenre.HasValue || !userSexualPreferences.HasValue)
            {
                // Si aucune préférence n'est définie, retourner une liste vide
                return userProfiles;
            }

            // Vérifier qu'au moins un critère de recherche est spécifié
            bool hasSearchCriteria = minAge.HasValue || maxAge.HasValue || 
                                    minPopularity.HasValue || maxPopularity.HasValue || 
                                    maxDistance.HasValue || (tagIds != null && tagIds.Any());

            if (!hasSearchCriteria)
            {
                // Si aucun critère n'est spécifié, retournez une liste vide
                // Nous exigeons au moins un critère spécifique en plus des préférences sexuelles
                return userProfiles;
            }

            // Construire la requête de base avec une jointure potentielle pour les tags
            var baseQueryBuilder = new StringBuilder();
            
            // Si nous avons des tags à filtrer, nous utilisons une requête avec jointure pour chercher dans les tags multiples
            if (tagIds != null && tagIds.Any())
            {
                baseQueryBuilder.Append(@"
                SELECT DISTINCT
                    u.id, u.username, u.firstname, u.lastname, u.email, u.age,
                    u.biography, u.genre_id, u.tag_id, u.sexual_preferences_id,
                    ST_X(u.gps_location) AS longitude, ST_Y(u.gps_location) AS latitude,
                    u.gps_location, -- Ajout pour ORDER BY
                    u.popularity_score, u.profile_complete, u.isactive,
                    u.localisation_isactive, u.notifisactive, u.created_at
                FROM 
                    users u
                LEFT JOIN 
                    userTags ut ON u.id = ut.user_id");
            }
            else
            {
                baseQueryBuilder.Append(@"
                SELECT 
                    u.id, u.username, u.firstname, u.lastname, u.email, u.age,
                    u.biography, u.genre_id, u.tag_id, u.sexual_preferences_id,
                    ST_X(u.gps_location) AS longitude, ST_Y(u.gps_location) AS latitude,
                    u.popularity_score, u.profile_complete, u.isactive,
                    u.localisation_isactive, u.notifisactive, u.created_at
                FROM 
                    users u");
            }
            
            // Construction des clauses WHERE
            baseQueryBuilder.Append(@"
                WHERE 
                    u.id != @UserId AND
                    u.profile_complete = 1 AND
                    u.isactive = 1");

            // Appliquer les filtres de compatibilité sexuelle (obligatoires)
            if (userGenre == 1) // Homme
            {
                if (userSexualPreferences == 1) // Hétéro
                {
                    // Montre uniquement les femmes hétéro
                    baseQueryBuilder.Append(" AND u.genre_id = 2 AND u.sexual_preferences_id = 1");
                }
                else if (userSexualPreferences == 2) // Gay
                {
                    // Montre uniquement les hommes gay
                    baseQueryBuilder.Append(" AND u.genre_id = 1 AND u.sexual_preferences_id = 2");
                }
                else if (userSexualPreferences == 3) // Bisexuel
                {
                    // Pour bisexuel, on montre les deux genres mais on filtre quand même par préférence
                    baseQueryBuilder.Append(" AND ((u.genre_id = 1 AND u.sexual_preferences_id IN (2, 3)) OR (u.genre_id = 2 AND u.sexual_preferences_id IN (1, 3)))");
                }
            }
            else if (userGenre == 2) // Femme
            {
                if (userSexualPreferences == 1) // Hétéro
                {
                    // Montre uniquement les hommes hétéro
                    baseQueryBuilder.Append(" AND u.genre_id = 1 AND u.sexual_preferences_id = 1");
                }
                else if (userSexualPreferences == 2) // Gay
                {
                    // Montre uniquement les femmes gay
                    baseQueryBuilder.Append(" AND u.genre_id = 2 AND u.sexual_preferences_id = 2");
                }
                else if (userSexualPreferences == 3) // Bisexuel
                {
                    // Pour bisexuel, on montre les deux genres mais on filtre quand même par préférence
                    baseQueryBuilder.Append(" AND ((u.genre_id = 1 AND u.sexual_preferences_id IN (1, 3)) OR (u.genre_id = 2 AND u.sexual_preferences_id IN (2, 3)))");
                }
            }
            else if (userGenre == 3) // Autre
            {
                // Pour le genre "Autre", on filtre quand même selon les préférences
                if (userSexualPreferences == 1) // Hétéro
                {
                    baseQueryBuilder.Append(" AND ((u.genre_id = 1 AND u.sexual_preferences_id = 1) OR (u.genre_id = 2 AND u.sexual_preferences_id = 1))");
                }
                else if (userSexualPreferences == 2) // Gay
                {
                    baseQueryBuilder.Append(" AND ((u.genre_id = 1 AND u.sexual_preferences_id = 2) OR (u.genre_id = 2 AND u.sexual_preferences_id = 2))");
                }
                else if (userSexualPreferences == 3) // Bisexuel
                {
                    baseQueryBuilder.Append(" AND u.sexual_preferences_id = 3");
                }
            }

            // Construire la clause AND pour exiger qu'au moins un critère de recherche corresponde
            baseQueryBuilder.Append(" AND (");
            
            List<string> conditions = new List<string>();
            
            // Ajouter chaque condition comme une option
            if (minAge.HasValue && maxAge.HasValue)
                conditions.Add("u.age BETWEEN @MinAge AND @MaxAge");
            else if (minAge.HasValue)
                conditions.Add("u.age >= @MinAge");
            else if (maxAge.HasValue)
                conditions.Add("u.age <= @MaxAge");
                
            if (minPopularity.HasValue && maxPopularity.HasValue)
                conditions.Add("u.popularity_score BETWEEN @MinPopularity AND @MaxPopularity");
            else if (minPopularity.HasValue)
                conditions.Add("u.popularity_score >= @MinPopularity");
            else if (maxPopularity.HasValue)
                conditions.Add("u.popularity_score <= @MaxPopularity");
                
            if (maxDistance.HasValue && userLatitude.HasValue && userLongitude.HasValue)
            {
                var maxDistanceMeters = maxDistance.Value * 1000;
                conditions.Add("ST_Distance_Sphere(POINT(@UserLongitude, @UserLatitude), u.gps_location) <= @MaxDistanceMeters");
            }
            
            // Condition spéciale pour les tags : vérifier si l'utilisateur a l'un des tags sélectionnés
            // soit comme tag principal (u.tag_id) soit comme tag supplémentaire (ut.tag_id)
            if (tagIds != null && tagIds.Any())
            {
                StringBuilder tagCondition = new StringBuilder("(");
                
                // Vérifier le tag principal
                tagCondition.Append("u.tag_id IN (");
                for (int i = 0; i < tagIds.Count; i++)
                {
                    if (i > 0) tagCondition.Append(",");
                    tagCondition.Append($"@Tag{i}");
                }
                tagCondition.Append(")");
                
                // OU vérifier les tags supplémentaires de l'utilisateur
                tagCondition.Append(" OR ut.tag_id IN (");
                for (int i = 0; i < tagIds.Count; i++)
                {
                    if (i > 0) tagCondition.Append(",");
                    tagCondition.Append($"@Tag{i}");
                }
                tagCondition.Append("))");
                
                conditions.Add(tagCondition.ToString());
            }
            
            // Joindre toutes les conditions avec OR pour exiger qu'au moins une corresponde
            baseQueryBuilder.Append(string.Join(" OR ", conditions));
            baseQueryBuilder.Append(")");
            
            // Trier par distance si disponible, sinon par popularité
            if (userLatitude.HasValue && userLongitude.HasValue)
            {
                baseQueryBuilder.Append(@" 
                ORDER BY 
                    ST_Distance_Sphere(POINT(@UserLongitude, @UserLatitude), u.gps_location) ASC");
            }
            else
            {
                baseQueryBuilder.Append(@" 
                ORDER BY 
                    u.popularity_score DESC");
            }
            
            baseQueryBuilder.Append(" LIMIT 50");

            using var command = new MySqlCommand(baseQueryBuilder.ToString(), connection);
            command.Parameters.AddWithValue("@UserId", userId);
            
            if (minAge.HasValue)
                command.Parameters.AddWithValue("@MinAge", minAge.Value);
                
            if (maxAge.HasValue)
                command.Parameters.AddWithValue("@MaxAge", maxAge.Value);
                
            if (minPopularity.HasValue)
                command.Parameters.AddWithValue("@MinPopularity", minPopularity.Value);
                
            if (maxPopularity.HasValue)
                command.Parameters.AddWithValue("@MaxPopularity", maxPopularity.Value);
                
            if (maxDistance.HasValue && userLatitude.HasValue && userLongitude.HasValue)
            {
                var maxDistanceMeters = maxDistance.Value * 1000;
                command.Parameters.AddWithValue("@UserLatitude", userLatitude.Value);
                command.Parameters.AddWithValue("@UserLongitude", userLongitude.Value);
                command.Parameters.AddWithValue("@MaxDistanceMeters", maxDistanceMeters);
            }
            
            if (tagIds != null && tagIds.Any())
            {
                for (int i = 0; i < tagIds.Count; i++)
                {
                    command.Parameters.AddWithValue($"@Tag{i}", tagIds[i]);
                }
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                userProfiles.Add(MapToUserProfileDto(reader));
            }

            return userProfiles;
        }

        // Méthode utilitaire pour mapper un MySqlDataReader à un UserProfileDto
        private UserProfileDto MapToUserProfileDto(DbDataReader reader)
        {
            return new UserProfileDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Username = reader.IsDBNull(reader.GetOrdinal("username")) ? string.Empty : reader.GetString(reader.GetOrdinal("username")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                Firstname = reader.IsDBNull(reader.GetOrdinal("firstname")) ? string.Empty : reader.GetString(reader.GetOrdinal("firstname")),
                Lastname = reader.IsDBNull(reader.GetOrdinal("lastname")) ? string.Empty : reader.GetString(reader.GetOrdinal("lastname")),
                Age = reader.GetInt32(reader.GetOrdinal("age")),
                Biography = reader.IsDBNull(reader.GetOrdinal("biography")) ? null : reader.GetString(reader.GetOrdinal("biography")),
                Genre = reader.IsDBNull(reader.GetOrdinal("genre_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("genre_id")),
                Tag = reader.IsDBNull(reader.GetOrdinal("tag_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("tag_id")),
                SexualPreferences = reader.IsDBNull(reader.GetOrdinal("sexual_preferences_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("sexual_preferences_id")),
                Latitude = reader.IsDBNull(reader.GetOrdinal("latitude")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("latitude")),
                Longitude = reader.IsDBNull(reader.GetOrdinal("longitude")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("longitude")),
                PopularityScore = reader.GetInt32(reader.GetOrdinal("popularity_score")),
                ProfileComplete = reader.GetBoolean(reader.GetOrdinal("profile_complete")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("isactive")),
                LocalisationIsActive = reader.GetBoolean(reader.GetOrdinal("localisation_isactive")),
                NotifIsActive = reader.GetBoolean(reader.GetOrdinal("notifisactive"))
            };
        }

        // Méthode utilitaire pour récupérer l'âge d'un utilisateur
        private async Task<int> GetUserAgeAsync(int userId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT age FROM users WHERE id = @UserId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
} 