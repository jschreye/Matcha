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
            // Construire l'entité Photo
            var photo = new Photo
            {
                UserId = userId,
                ImageData = imageData,
                EstProfil = estProfil
            };

            // Appeler le repository pour insérer la photo
            await _photoRepository.InsertPhotoAsync(photo);
        }   
    }
}