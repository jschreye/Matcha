using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetGenresAsync();
    }
}