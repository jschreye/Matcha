using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface IPhotoRepository
    {
        Task<int> InsertPhotoAsync(Photo photo);
        Task<int> AddPhotoWithProfileAsync(Photo photo);
        Task<Photo?> GetProfilePhotoAsync(int userId);
        Task DeletePhotoAsync(int photoId);
        Task<List<Photo>> GetUserPhotosAsync(int userId);
        Task UpdateProfilePhotoAsync(int userId, int newProfilePhotoId);
    }
}