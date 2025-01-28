using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;

namespace Infrastructure.Services
{
    public class PrefSexService : IPrefSexService
    {
        private readonly IPrefSexRepository _prefSexRepository;
        public PrefSexService(IPrefSexRepository prefSexRepository)
        {
            _prefSexRepository = prefSexRepository;
        } 
        public async Task<List<PrefSex>> GetPrefSexAsync()
        {
            return await _prefSexRepository.GetPrefSexAsync();
        }      
    }
}