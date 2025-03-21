
namespace Infrastructure.Services
{
    public interface ILikeService
    {
        Task LikeProfileAsync(int userId, int likedUserId);
        Task UnlikeProfileAsync(int userId, int likedUserId);
    }
}