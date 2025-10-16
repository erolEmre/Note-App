using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Services
{
    public interface ITagService
    {
        public Task UpdateTagCount(int TagId,TagUpdateStatus status);
        public Task<Tag> GetTagByName(string TagName);
    }
}
