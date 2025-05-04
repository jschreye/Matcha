namespace Core.Interfaces.Repository
{
    public interface IBlockReportRepository
    {
        Task AddBlockAsync(int userId, int blockedUserId);
        Task RemoveBlockAsync(int userId, int blockedUserId);
        Task<bool> IsBlockedAsync(int userId, int otherUserId);
        Task AddReportAsync(int userId, int reportedUserId, string reason);
        Task<List<(int ReportedUserId, string Reason, DateTime Timestamp)>> GetReportsByUserAsync(int userId);
        Task<bool> HasBlockBetweenAsync(int userId1, int userId2);
    }
}