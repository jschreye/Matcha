using Core.Data.Entity;

namespace Core.Interfaces.Services
{
    public interface IPrefSexService
    {
        Task<List<PrefSex>> GetPrefSexAsync();
    }
}