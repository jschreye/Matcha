using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data.Entity;
using Core.Interfaces.Services;
using Core.Interfaces.Repository;

namespace Infrastructure.Services
{
    public class BlockReportService : IBlockReportService
    {
        private readonly IBlockReportRepository _repo;
        public BlockReportService(IBlockReportRepository repo) => _repo = repo;

        public Task BlockUserAsync(int userId, int blockedUserId)
            => _repo.AddBlockAsync(userId, blockedUserId);

        public Task UnblockUserAsync(int userId, int blockedUserId)
            => _repo.RemoveBlockAsync(userId, blockedUserId);

        public Task<bool> IsBlockedAsync(int userId, int otherUserId)
            => _repo.IsBlockedAsync(userId, otherUserId);

        public Task ReportUserAsync(int userId, int reportedUserId, string reason)
            => _repo.AddReportAsync(userId, reportedUserId, reason);

        public Task<List<(int, string, DateTime)>> GetMyReportsAsync(int userId)
            => _repo.GetReportsByUserAsync(userId);
        
        public async Task<bool> HasBlockBetweenAsync(int userId1, int userId2)
        {
            return await _repo.HasBlockBetweenAsync(userId1, userId2);
        }
    }
}