using Core.Data.DTOs;

namespace Core.Interfaces.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Recherche des profils similaires à l'utilisateur actuel
        /// </summary>
        /// <param name="userId">ID de l'utilisateur actuel</param>
        /// <returns>Liste des profils similaires</returns>
        Task<List<UserProfileDto>> FindSimilarProfilesAsync(int userId);
        
        /// <summary>
        /// Recherche aléatoire de profils
        /// </summary>
        /// <param name="userId">ID de l'utilisateur actuel (à exclure des résultats)</param>
        /// <param name="count">Nombre de profils à retourner</param>
        /// <returns>Liste aléatoire de profils</returns>
        Task<List<UserProfileDto>> FindRandomProfilesAsync(int userId, int count = 10);
        
        /// <summary>
        /// Recherche de profils selon des critères personnalisés
        /// </summary>
        /// <param name="userId">ID de l'utilisateur actuel (à exclure des résultats)</param>
        /// <param name="minAge">Âge minimum</param>
        /// <param name="maxAge">Âge maximum</param>
        /// <param name="minPopularity">Score de popularité minimum</param>
        /// <param name="maxPopularity">Score de popularité maximum</param>
        /// <param name="maxDistance">Distance maximale en kilomètres</param>
        /// <param name="tagIds">Liste des IDs de tags à rechercher</param>
        /// <returns>Liste des profils correspondant aux critères</returns>
        Task<List<UserProfileDto>> FindCustomProfilesAsync(
            int userId,
            int? minAge = null,
            int? maxAge = null,
            int? minPopularity = null,
            int? maxPopularity = null,
            double? maxDistance = null,
            List<int>? tagIds = null);
    }
} 