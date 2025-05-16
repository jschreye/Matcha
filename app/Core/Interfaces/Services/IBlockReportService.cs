using Core.Data.Entity;

namespace Core.Interfaces.Services
{
    public interface IBlockReportService
    {
        Task BlockUserAsync(int userId, int blockedUserId);
        Task UnblockUserAsync(int userId, int blockedUserId);
        Task<bool> IsBlockedAsync(int userId, int otherUserId);
        Task ReportUserAsync(int userId, int reportedUserId, string reason);
        Task<List<(int ReportedUserId, string Reason, DateTime Timestamp)>> GetMyReportsAsync(int userId);
        Task<bool> HasBlockBetweenAsync(int userId1, int userId2);
    }
}