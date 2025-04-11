
namespace Core.Interfaces.Services
{
    public interface ILikeService
    {
        Task LikeProfileAsync(int userId, int likedUserId);
        Task UnlikeProfileAsync(int userId, int likedUserId);
        Task<bool> HasLikedAsync(int userId, int likedUserId);
    }
}