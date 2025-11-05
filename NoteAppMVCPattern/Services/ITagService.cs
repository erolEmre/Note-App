using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Services
{
    public interface ITagService
    {
        public Task UpdateTagCount(int TagId,TagUpdateStatus status);
        public Task<Tag> GetTagByName(string TagName);

        public Task<List<Tag>> GetTags(string userId);
        public Task AddToExistingTag(int noteId, int tag, string userId);
        public Task DeleteTag(int noteId, string userId, int tagId);
        public Task CreateAndAdd(int noteId, string tagName, string userId);

        public Task<Tag> GetTag(int tagId);
        public Task<List<Tag>> GetTagByNote (int noteId);
        public void SaveChanges();
    }
}
