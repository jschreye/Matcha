using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;

namespace Infrastructure.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IMatchRepository _matchRepository;
        public LikeService(ILikeRepository likeRepository, IMatchRepository matchRepository)
        {
            _likeRepository = likeRepository;
            _matchRepository = matchRepository;
        }
        public async Task<bool> LikeProfileAsync(int userId, int likedUserId)
        {
            await _likeRepository.LikeProfileAsync(userId, likedUserId);

            var hasLikedBack = await _likeRepository.HasLikedBackAsync(likedUserId, userId);

            if (hasLikedBack)
            {
                await _matchRepository.CreateMatchAsync(userId, likedUserId);
                Console.WriteLine("🔥 It's a match!");
                return true;
            }
            return false;
        }
        public async Task UnlikeProfileAsync(int userId, int likedUserId)
        {
            await _likeRepository.UnlikeProfileAsync(userId, likedUserId);

            await _matchRepository.DeleteMatchAsync(userId, likedUserId);
        }
        
        public async Task<bool> HasLikedAsync(int userId, int likedUserId)
        {
            return await _likeRepository.HasLikedAsync(userId, likedUserId);
        }
    }
}