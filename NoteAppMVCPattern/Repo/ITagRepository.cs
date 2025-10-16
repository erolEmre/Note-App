using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Repo
{
    public interface ITagRepository
    {
        public Task UpdateTagCountAsync(int tagId,TagUpdateStatus status);

        public Task<Tag> GetTagByName(string tagName);

    }
}
