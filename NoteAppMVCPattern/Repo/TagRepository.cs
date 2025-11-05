using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Services;

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
            } if (TagUpdateStatus.Decrement == status)
            {
                if(tag.TagUsageCount > 0 && tag.TagUsageCount != 1)
                tag.TagUsageCount--;
                else
                {
                    tag.TagUsageCount--;
                    _dbContext.Tag.Remove(tag);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Tag> GetTagByName(string tagName)
        {
            return await _dbContext.Tag.FirstOrDefaultAsync(x => x.TagName == tagName);
        }
        public async Task<List<Tag>> GetTags(string userId)
        {
            return await _dbContext.Notes
                .Include(n => n.Tags)
                .Where(n => n.UserId == userId)
                .SelectMany(n => n.Tags)
                .Distinct()
                .ToListAsync();
        }

        public Tag GetTag(int tagId)
        {
            return _dbContext.Tag
                 .FirstOrDefault(t => t.Id == tagId);

        }
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public async Task<List<Tag>> GetTagsByNote(int noteId)
        {
            var myNote = _dbContext.Notes.FirstOrDefault(x=> x.Id == noteId);
            return myNote.Tags.ToList();
        }
    }
}
