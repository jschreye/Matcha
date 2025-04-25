using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;

namespace Infrastructure.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository      _likeRepository;
        private readonly IMatchRepository     _matchRepository;
        private readonly IUserRepository      _userRepository;
        private readonly INotificationService _notificationService;

        public LikeService(
            ILikeRepository likeRepository,
            IMatchRepository matchRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _likeRepository      = likeRepository;
            _matchRepository     = matchRepository;
            _userRepository      = userRepository;
            _notificationService = notificationService;
        }

        public async Task<bool> LikeProfileAsync(int userId, int likedUserId)
        {
            await _likeRepository.LikeProfileAsync(userId, likedUserId);

            await _userRepository.ChangePopularityAsync(likedUserId, +1);

            await _notificationService.NotifyProfileLikedAsync(likedUserId, userId);

            var hasLikedBack = await _likeRepository.HasLikedBackAsync(likedUserId, userId);
            if (hasLikedBack)
            {
                await _matchRepository.CreateMatchAsync(userId, likedUserId);

                await _userRepository.ChangePopularityAsync(userId,       +3);
                await _userRepository.ChangePopularityAsync(likedUserId, +3);

                await _notificationService.NotifyMatchAsync(userId,       likedUserId);
                await _notificationService.NotifyMatchAsync(likedUserId,  userId);

                return true;
            }

            return false;
        }

        public async Task UnlikeProfileAsync(int userId, int likedUserId)
        {
            await _likeRepository.UnlikeProfileAsync(userId, likedUserId);

            var matchedIds = await _matchRepository.GetMatchedUserIdsAsync(userId);
            var hadMatch   = matchedIds.Contains(likedUserId);

            await _likeRepository.UnlikeProfileAsync(userId, likedUserId);

            await _userRepository.ChangePopularityAsync(likedUserId, -1);

            if (hadMatch)
            {
                await _matchRepository.DeleteMatchAsync(userId, likedUserId);
                await _userRepository.ChangePopularityAsync(userId, -3);
                await _userRepository.ChangePopularityAsync(likedUserId, -3);
            }
        }
        
        public async Task<bool> HasLikedAsync(int userId, int likedUserId)
        {
            return await _likeRepository.HasLikedAsync(userId, likedUserId);
        }
    }
}