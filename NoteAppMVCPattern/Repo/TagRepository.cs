using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Repo
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDBContext _dbContext;
        public TagRepository(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateTagCountAsync(int tagId, TagUpdateStatus status)
        {
            var tag = await _dbContext.Tag.FindAsync(tagId);
            if (tag == null) return;
            if (TagUpdateStatus.Increment == status)
            {
                tag.TagUsageCount++;
            } if (TagUpdateStatus.Decrement == status && tag.TagUsageCount > 0)
            {
                tag.TagUsageCount--;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Tag> GetTagByName(string tagName)
        {
            return await _dbContext.Tag.FirstOrDefaultAsync(x => x.TagName == tagName);
        }
    }
}
