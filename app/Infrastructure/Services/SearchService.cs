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
        private readonly ISearchRepository _searchRepository;
        
        public SearchService(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        public async Task<List<UserProfileDto>> FindSimilarProfilesAsync(int userId)
        {
            return await _searchRepository.GetSimilarProfilesAsync(userId);
        }

        public async Task<List<UserProfileDto>> FindRandomProfilesAsync(int userId, int count = 10)
        {
            return await _searchRepository.GetRandomProfilesAsync(userId, count);
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
            return await _searchRepository.GetFilteredProfilesAsync(
                userId,
                minAge,
                maxAge,
                minPopularity,
                maxPopularity,
                maxDistance,
                tagIds);
        }
    }
} 