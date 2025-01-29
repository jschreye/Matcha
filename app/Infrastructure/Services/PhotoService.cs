using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;

namespace Infrastructure.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _photoRepository;

        public PhotoService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task AddPhotoAsync(int userId, byte[] imageData, bool estProfil)
        {
            var photo = new Photo
            {
                UserId = userId,
                ImageData = imageData,
                EstProfil = estProfil
            };

            if (estProfil)
            {
                await _photoRepository.AddPhotoWithProfileAsync(photo);
            }
            else
            {
                await _photoRepository.InsertPhotoAsync(photo);
            }
        }

        public async Task<Photo?> GetProfilePhotoAsync(int userId)
        {
            return await _photoRepository.GetProfilePhotoAsync(userId);
        }
        public async Task<List<Photo>> GetUserPhotosAsync(int userId)
        {
            return await _photoRepository.GetUserPhotosAsync(userId);
        }
    }
}