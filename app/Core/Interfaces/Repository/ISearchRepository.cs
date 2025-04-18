using Core.Data.DTOs;

namespace Core.Interfaces.Repository
{
    public interface ISearchRepository
    {
        /// <summary>
        /// Récupère les profils similaires à un utilisateur spécifique
        /// </summary>
        /// <param name="userId">ID de l'utilisateur de référence</param>
        /// <returns>Liste des profils similaires</returns>
        Task<List<UserProfileDto>> GetSimilarProfilesAsync(int userId);
        
        /// <summary>
        /// Récupère des profils aléatoires, en excluant l'utilisateur actuel
        /// </summary>
        /// <param name="userId">ID de l'utilisateur à exclure</param>
        /// <param name="count">Nombre de profils à récupérer</param>
        /// <returns>Liste aléatoire de profils</returns>
        Task<List<UserProfileDto>> GetRandomProfilesAsync(int userId, int count);
        
        /// <summary>
        /// Récupère des profils selon des critères spécifiques
        /// </summary>
        /// <param name="userId">ID de l'utilisateur à exclure</param>
        /// <param name="minAge">Âge minimum</param>
        /// <param name="maxAge">Âge maximum</param>
        /// <param name="minPopularity">Score de popularité minimum</param>
        /// <param name="maxPopularity">Score de popularité maximum</param>
        /// <param name="maxDistance">Distance maximale en kilomètres</param>
        /// <param name="tagIds">Liste des IDs de tags</param>
        /// <returns>Liste des profils correspondant aux critères</returns>
        Task<List<UserProfileDto>> GetFilteredProfilesAsync(
            int userId,
            int? minAge,
            int? maxAge,
            int? minPopularity,
            int? maxPopularity,
            double? maxDistance,
            List<int>? tagIds);
    }
} 