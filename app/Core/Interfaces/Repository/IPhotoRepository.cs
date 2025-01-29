using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface IPhotoRepository
    {
        Task InsertPhotoAsync(Photo photo);
    }
}