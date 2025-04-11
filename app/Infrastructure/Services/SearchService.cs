using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.DTOs;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly ITagService _tagService;
        
        public SearchService(IUserRepository userRepository, IUserService userService, ITagService tagService)
        {
            _userRepository = userRepository;
            _userService = userService;
            _tagService = tagService;
        }

        public async Task<List<UserProfileDto>> FindSimilarProfilesAsync(int userId)
        {
            // Récupérer le profil de l'utilisateur actuel
            var currentUser = await _userService.GetUserProfileAsync(userId);
            if (currentUser == null)
            {
                return new List<UserProfileDto>();
            }

            // Récupérer tous les utilisateurs
            var allUsers = await _userRepository.GetAllUserAsync();
            
            // Convertir les UserDto en UserProfileDto
            var userProfiles = new List<UserProfileDto>();
            foreach (var user in allUsers)
            {
                if (user.Id == userId) continue; // Exclure l'utilisateur actuel
                
                var profile = await _userService.GetUserProfileAsync(user.Id);
                if (profile != null)
                {
                    userProfiles.Add(profile);
                }
            }

            // Calculer un score de similarité pour chaque profil
            var scoredProfiles = userProfiles.Select(profile => new
            {
                Profile = profile,
                // Calculer un score de similarité basé sur différents critères
                SimilarityScore = CalculateSimilarityScore(currentUser, profile)
            })
            .OrderByDescending(p => p.SimilarityScore)
            .Take(10) // Limiter à 10 résultats
            .Select(p => p.Profile)
            .ToList();

            return scoredProfiles;
        }

        public async Task<List<UserProfileDto>> FindRandomProfilesAsync(int userId, int count = 10)
        {
            // Récupérer tous les utilisateurs
            var allUsers = await _userRepository.GetAllUserAsync();
            
            // Convertir les UserDto en UserProfileDto et exclure l'utilisateur actuel
            var userProfiles = new List<UserProfileDto>();
            foreach (var user in allUsers)
            {
                if (user.Id == userId) continue; // Exclure l'utilisateur actuel
                
                var profile = await _userService.GetUserProfileAsync(user.Id);
                if (profile != null)
                {
                    userProfiles.Add(profile);
                }
            }

            // Mélanger la liste et prendre un nombre limité d'éléments
            var random = new Random();
            var randomProfiles = userProfiles
                .OrderBy(x => random.Next())
                .Take(count)
                .ToList();

            return randomProfiles;
        }

        public async Task<List<UserProfileDto>> FindCustomProfilesAsync(
            int userId,
            int? minAge = null,
            int? maxAge = null,
            int? minPopularity = null,
            int? maxPopularity = null,
            double? maxDistance = null,
            List<int>? tagIds = null)
        {
            // Récupérer l'utilisateur actuel (pour les calculs de distance)
            var currentUser = await _userService.GetUserProfileAsync(userId);
            if (currentUser == null)
            {
                return new List<UserProfileDto>();
            }

            // Récupérer tous les utilisateurs
            var allUsers = await _userRepository.GetAllUserAsync();
            
            // Convertir les UserDto en UserProfileDto
            var userProfiles = new List<UserProfileDto>();
            foreach (var user in allUsers)
            {
                if (user.Id == userId) continue; // Exclure l'utilisateur actuel
                
                var profile = await _userService.GetUserProfileAsync(user.Id);
                if (profile != null)
                {
                    userProfiles.Add(profile);
                }
            }

            // Filtrer les profils selon les critères
            var filteredProfiles = userProfiles.Where(profile =>
                (!minAge.HasValue || profile.Age >= minAge.Value) &&
                (!maxAge.HasValue || profile.Age <= maxAge.Value) &&
                (!minPopularity.HasValue || profile.PopularityScore >= minPopularity.Value) &&
                (!maxPopularity.HasValue || profile.PopularityScore <= maxPopularity.Value) &&
                (!maxDistance.HasValue || 
                    !currentUser.Latitude.HasValue || 
                    !currentUser.Longitude.HasValue || 
                    !profile.Latitude.HasValue || 
                    !profile.Longitude.HasValue ||
                    CalculateDistance(
                        currentUser.Latitude.Value, 
                        currentUser.Longitude.Value, 
                        profile.Latitude.Value, 
                        profile.Longitude.Value) <= maxDistance.Value)
            ).ToList();

            // Si des tags sont spécifiés, filtrer par tags (à implémenter selon le modèle de données des tags)
            if (tagIds != null && tagIds.Any())
            {
                filteredProfiles = filteredProfiles.Where(profile => 
                    profile.Tag.HasValue && tagIds.Contains(profile.Tag.Value)
                ).ToList();
            }

            return filteredProfiles;
        }

        /// <summary>
        /// Calcule un score de similarité entre deux profils
        /// </summary>
        private double CalculateSimilarityScore(UserProfileDto user1, UserProfileDto user2)
        {
            double score = 0;

            // Similarité d'âge (plus ils sont proches, plus le score est élevé)
            var ageDiff = Math.Abs(user1.Age - user2.Age);
            if (ageDiff <= 5) score += 10;
            else if (ageDiff <= 10) score += 5;
            
            // Similarité de tags (si les deux utilisateurs ont le même tag)
            if (user1.Tag.HasValue && user2.Tag.HasValue && user1.Tag.Value == user2.Tag.Value)
            {
                score += 20;
            }

            // Préférences sexuelles correspondantes
            if (user1.SexualPreferences.HasValue && user2.Genre.HasValue && 
                user1.SexualPreferences.Value == user2.Genre.Value)
            {
                score += 15;
            }

            // Critère de proximité géographique
            if (user1.Latitude.HasValue && user1.Longitude.HasValue && 
                user2.Latitude.HasValue && user2.Longitude.HasValue)
            {
                var distance = CalculateDistance(
                    user1.Latitude.Value, user1.Longitude.Value,
                    user2.Latitude.Value, user2.Longitude.Value);
                
                // Plus ils sont proches, plus le score est élevé
                if (distance <= 5) score += 15;
                else if (distance <= 20) score += 10;
                else if (distance <= 50) score += 5;
            }

            return score;
        }

        /// <summary>
        /// Calcule la distance entre deux points géographiques (formule de Haversine)
        /// </summary>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371.0;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = earthRadiusKm * c;

            return distance;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
} 