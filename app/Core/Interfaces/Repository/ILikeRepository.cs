using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface ILikeRepository
    {
        Task LikeProfileAsync(int userId, int likedUserId);
        Task UnlikeProfileAsync(int userId, int likedUserId);
        Task<bool> HasLikedAsync(int userId, int likedUserId);
    }
}