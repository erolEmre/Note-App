using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Repo.Tags
{
    public interface ITagRepository
    {
        public Task UpdateTagCountAsync(int tagId,TagUpdateStatus status);

        public Task<Tag> GetTagByName(string tagName);
        Task<List<Tag>> GetTags(string userId);
        Tag GetTag(int tagId);
        public void SaveChanges();

    }
}
