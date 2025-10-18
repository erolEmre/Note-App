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
       
        public async Task<List<Note>> GetAllByUserIdAsync(string userId, List<int> tagIds = null, string sortOrder = null)
        {
            // Notları kullanıcıya göre filtrele ve Tag ilişkisini dahil et
            var notesQuery = _dbContext.Notes
                .Include(n => n.Tags)
                .Where(n => n.UserId == userId);

            // Etiket filtreleme
            if (tagIds != null && tagIds.Any())
            {
                notesQuery = notesQuery.Where(n => n.Tags.Any(t => tagIds.Contains(t.Id)));
            }

            // Sıralama mantığı
            notesQuery = sortOrder switch
            {
                "date_asc" => notesQuery.OrderBy(n => n.updatedDate),
                "date_desc" => notesQuery.OrderByDescending(n => n.updatedDate),
                _ => notesQuery.OrderByDescending(n => n.updatedDate)
            };

            return await notesQuery.ToListAsync();
        }

        public async Task<Note> GetByIdAsync(int id, string userId)
        {
            // Tag’lerle birlikte getiriyoruz ki ilişkili veriler boş kalmasın
            return await _dbContext.Notes
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }

        public async Task Add(Note note)
        {
            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Note note)
        {
            _dbContext.Notes.Update(note);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Note note)
        {
            _dbContext.Notes.Remove(note);
            await _dbContext.SaveChangesAsync();
        }

        
    }

}
