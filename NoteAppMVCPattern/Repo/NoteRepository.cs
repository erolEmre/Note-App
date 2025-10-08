using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;
using System.Threading.Tasks;

namespace NoteAppMVCPattern.Repo
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDBContext _dbContext;
        public NoteRepository(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }
            
        public async Task<List<Note>> GetAllByUserIdAsync(string userId, string tag = null, string sortOrder = null)
        {
            var notesQuery = _dbContext.Notes.Where(n => n.UserId == userId);

            // Eğer bir etiket filtresi varsa, uygula.
            if (!string.IsNullOrEmpty(tag))
            {
                notesQuery = notesQuery.Where(n => n.Tag == tag);
            }

            // Sıralama mantığını uygula.
            notesQuery = sortOrder switch
            {
                "date_asc" => notesQuery.OrderBy(n => n.updatedDate),
                "date_desc" => notesQuery.OrderByDescending(n => n.updatedDate),
                _ => notesQuery.OrderByDescending(n => n.updatedDate) // Varsayılan sıralama
            };

            return await notesQuery.ToListAsync();
        }

        public async Task<Note> GetByIdAsync(int id, string userId)
        {
            return await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task<List<string>> GetTags(string userId)
        {
            return await _dbContext.Notes.Where(x=> x.UserId == userId && x.Tag != null)
                .Select(x => x.Tag)
                .Distinct()
                .ToListAsync();
        }

        public async Task Add(Note note)
        {
            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Note note)
        {
            _dbContext.Notes.Remove(note);
            await _dbContext.SaveChangesAsync();
        }

       public async Task Update(Note note)
        {
            _dbContext.Notes.Update(note);
            await _dbContext.SaveChangesAsync();
        }
    }
}
