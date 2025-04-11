
namespace Core.Interfaces.Repository
{
    public interface IVisitRepository
    {
        Task<List<int>> GetProfileVisitorsIdsAsync(int userId);
        Task<List<int>> GetVisitedProfileIdsAsync(int userId);
    }
}