
namespace Core.Interfaces.Services
{
    public interface ILikeService
    {
        Task<bool> LikeProfileAsync(int userId, int likedUserId);
        Task UnlikeProfileAsync(int userId, int likedUserId);
        Task<bool> HasLikedAsync(int userId, int likedUserId);
    }
}