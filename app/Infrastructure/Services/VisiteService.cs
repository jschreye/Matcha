using Core.Interfaces.Repository;
using Core.Interfaces.Services;
using Core.Data.Entity;

namespace Infrastructure.Services
{
    public class VisiteService : IVisitService
    {
        private readonly IVisitRepository _visitRepository;
        private readonly IUserRepository  _userRepository;

        public VisiteService(
            IVisitRepository visitRepository,
            IUserRepository  userRepository)
        {
            _visitRepository = visitRepository;
            _userRepository  = userRepository;
        }

        public async Task RecordVisitAsync(int visitorId, int visitedId)
        {
            if (visitorId == visitedId)
                return;

            await _visitRepository.AddVisitAsync(visitorId, visitedId);

            await _userRepository.ChangePopularityAsync(visitedId, +1);
        }

        public Task<List<int>> GetVisitedProfilesAsync(int userId)
            => _visitRepository.GetVisitedProfileIdsAsync(userId);

        public Task<List<int>> GetProfileVisitorsAsync(int userId)
            => _visitRepository.GetProfileVisitorsIdsAsync(userId);
    }
}
