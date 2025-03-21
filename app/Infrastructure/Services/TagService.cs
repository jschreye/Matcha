using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.Entity;

namespace Infrastructure.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            return await _tagRepository.GetTagsAsync();
        }

        public async Task SaveUserTagsAsync(int userId, List<int> tagIds)
        {
            await _tagRepository.SaveUserTagsAsync(userId, tagIds);
        }

        public async Task<List<int>> GetTagUserAsync(int userId)
        {
            return await _tagRepository.GetTagUserAsync(userId);
        }
    }
}