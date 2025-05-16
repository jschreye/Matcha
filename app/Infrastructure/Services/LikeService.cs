using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;

namespace Infrastructure.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository  _likeRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly IUserRepository  _userRepo;

        public LikeService(
            ILikeRepository likeRepository,
            IMatchRepository matchRepository,
            IUserRepository userRepository)
        {
            _likeRepo  = likeRepository;
            _matchRepo = matchRepository;
            _userRepo  = userRepository;
        }

        /// <summary>
        /// Renvoie true si un tout nouveau match vient d’être créé.
        /// </summary>
        public async Task<bool> LikeProfileAsync(int userId, int likedUserId)
        {
            // 1) ajouter le like
            await _likeRepo.LikeProfileAsync(userId, likedUserId);

            // 2) popularité +1
            await _userRepo.ChangePopularityAsync(likedUserId, +1);

            // 3) si like réciproque, tenter de créer le match
            var hasLikedBack = await _likeRepo.HasLikedBackAsync(likedUserId, userId);
            if (!hasLikedBack)
                return false;

            // 4) créer / réactiver le match, et savoir si c’est vraiment nouveau
            var isNewMatch = await _matchRepo.CreateMatchAsync(userId, likedUserId);
            if (isNewMatch)
            {
                // popularité +3 chacun
                await _userRepo.ChangePopularityAsync(userId,       +3);
                await _userRepo.ChangePopularityAsync(likedUserId, +3);
            }

            return isNewMatch;
        }

        public async Task UnlikeProfileAsync(int userId, int likedUserId)
        {
            // 1) y avait-il un match ?
            var hadMatch = (await _matchRepo.GetMatchedUserIdsAsync(userId))
                            .Contains(likedUserId);

            // 2) supprimer le like
            await _likeRepo.UnlikeProfileAsync(userId, likedUserId);

            // 3) popularité –1
            await _userRepo.ChangePopularityAsync(likedUserId, -1);

            if (hadMatch)
            {
                // 4) supprimer le match + popularité –3 chacun
                await _matchRepo.DeleteMatchAsync(userId, likedUserId);
                await _userRepo.ChangePopularityAsync(userId,       -3);
                await _userRepo.ChangePopularityAsync(likedUserId, -3);
            }
        }

        public Task<bool> HasLikedAsync(int userId, int likedUserId)
            => _likeRepo.HasLikedAsync(userId, likedUserId);
    }
}