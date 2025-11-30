using Microsoft.EntityFrameworkCore;
using NoteApp.Infrastructure.Models;
using System.Threading.Tasks;
using NoteApp.Application.Repo.Notes;
using NoteApp.Core.Entities;

namespace NoteApp.Infrastructure.Services.Notes
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDBContext _dbContext;

        public NoteRepository(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }
               
        public async Task<Note?> GetByIdAsync(int id, string userId)
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
        
        public async Task<List<Note>> GetAllAsync()
        {
            return await _dbContext.Notes.ToListAsync();
        }

        public async Task<List<Note>> GetByNotebookIdAsync(int notebookId)
        {
            return await _dbContext.Notes
                .Include(n => n.Tags) // tags kullanacağımız için include burada olabilir
                .Where(n => n.NotebookId == notebookId)
                .ToListAsync();
        }


        //public async Task<List<Note>> GetAllByUserIdAsync(int notebookId, string userId, List<int> tagIds = null, string sortOrder = null)
        //{
        //    // Notları kullanıcıya göre filtrele ve Tag ilişkisini dahil et
        //    //var notebookQuery = _dbContext.Notebook.FirstOrDefault(x => x.Id == notebookId);

        //    var notesQuery = _dbContext.Notes.Where(x=> x.NotebookId == notebookId)
        //        .Include(n => n.Tags)
        //        .Where(n => n.UserId == userId);

        //    // Etiket filtreleme
        //    if (tagIds != null && tagIds.Any())
        //    {
        //        notesQuery = notesQuery.Where(n => n.Tags.Any(t => tagIds.Contains(t.Id)));
        //    }

        //    // Sıralama mantığı
        //    notesQuery = sortOrder switch
        //    {
        //        "date_asc" => notesQuery.OrderBy(n => n.updatedDate),
        //        "date_desc" => notesQuery.OrderByDescending(n => n.updatedDate),
        //        _ => notesQuery.OrderByDescending(n => n.updatedDate)
        //    };

        //    return await notesQuery.ToListAsync();
        //}
    }

}
