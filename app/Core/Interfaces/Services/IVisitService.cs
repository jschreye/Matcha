

namespace Core.Interfaces.Services
{
    public interface IVisitService
    {
        Task RecordVisitAsync(int visitorId, int visitedId);
        Task<List<int>> GetVisitedProfilesAsync(int userId);
        Task<List<int>> GetProfileVisitorsAsync(int userId);
    }
}
