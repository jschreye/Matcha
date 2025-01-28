using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface IPrefSexRepository
    {
        Task<List<PrefSex>> GetPrefSexAsync();
        Task<PrefSex?> GetPrefSexByIdAsync(int id);
    }
}