using Core.Data.Entity;

namespace Core.Interfaces.Services
{
    public interface IPhotoService
    {
        Task AddPhotoAsync(int userId, byte[] imageData, bool estProfil);
        Task<Photo?> GetProfilePhotoAsync(int userId);
        Task<List<Photo>> GetUserPhotosAsync(int userId);
        Task DeletePhotoAsync(int photoId);
        Task UpdateProfilePhotoAsync(int userId, int newProfilePhotoId);
    }
}