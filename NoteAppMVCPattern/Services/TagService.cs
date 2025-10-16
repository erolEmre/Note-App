using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Repo;

namespace NoteAppMVCPattern.Services
{
    public class TagService : ITagService
    {

        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task UpdateTagCount(int TagId,TagUpdateStatus status)
        {
            await _tagRepository.UpdateTagCountAsync(TagId,status);
        }

        public async Task<Tag> GetTagByName(string tagName)
        {
            var tag = await _tagRepository.GetTagByName(tagName);
            if (tag == null)
                throw new Exception("Tag bulunamadı.");
            return tag;
        }
    }
}
