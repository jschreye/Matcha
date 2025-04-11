using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;

namespace Infrastructure.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        public LikeService(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }
        public async Task LikeProfileAsync(int userId, int likedUserId)
        {
            await _likeRepository.LikeProfileAsync(userId, likedUserId);
        }
        public async Task UnlikeProfileAsync(int userId, int likedUserId)
        {
            await _likeRepository.UnlikeProfileAsync(userId, likedUserId);
        }
        public async Task<bool> HasLikedAsync(int userId, int likedUserId)
        {
            return await _likeRepository.HasLikedAsync(userId, likedUserId);
        }
    }
}