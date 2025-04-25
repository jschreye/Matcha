
using Core.Interfaces.Repository;
using Core.Interfaces.Services;

public class VisiteService : IVisitService
{
    private readonly IVisitRepository _visitRepository;

    public VisiteService(IVisitRepository visitRepository)
    {
        _visitRepository = visitRepository;
    }

    public Task RecordVisitAsync(int visitorId, int visitedId)
    {
        return _visitRepository.AddVisitAsync(visitorId, visitedId);
    }

    public Task<List<int>> GetVisitedProfilesAsync(int userId)
    {
        return _visitRepository.GetVisitedProfileIdsAsync(userId);
    }

    public Task<List<int>> GetProfileVisitorsAsync(int userId)
    {
        return _visitRepository.GetProfileVisitorsIdsAsync(userId);
    }
}
