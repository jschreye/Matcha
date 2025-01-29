using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface IPhotoRepository
    {
        Task InsertPhotoAsync(Photo photo);
        Task AddPhotoWithProfileAsync(Photo photo);
        Task<Photo?> GetProfilePhotoAsync(int userId);
        Task DeletePhotoAsync(int photoId);
        Task<List<Photo>> GetUserPhotosAsync(int userId);
    }
}