namespace Core.Interfaces.Services
{
    public interface IPhotoService
    {
        Task AddPhotoAsync(int userId, byte[] imageData, bool estProfil);
    }
}