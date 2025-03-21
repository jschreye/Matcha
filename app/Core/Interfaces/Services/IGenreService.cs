using Core.Data.Entity;

namespace Core.Interfaces.Services
{
    public interface IGenreService
    {
        Task<List<Genre>> GetGenresAsync();
        Task<Genre?> GetGenreByIdAsync(int id);
    }
}