using Microsoft.EntityFrameworkCore;
using NoteApp.Infrastructure.Models;
using NoteApp.Application.Repo.Notebooks;
using NoteApp.Core.Entities;

namespace NoteApp.Infrastructure.Repo.Notebooks
{

    public class NotebookRepository : INotebookRepository
    {
        private readonly AppDBContext _dbContext;
        public NotebookRepository(AppDBContext appDBContext)
        {
            _dbContext = appDBContext;
        }
        public async Task Add(Notebook notebook)
        {
            _dbContext.Notebook.Add(notebook);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Notebook notebook)
        {        
            _dbContext.Notebook.Remove(notebook);
            await _dbContext.SaveChangesAsync();                           
        }

        public async Task<int> EnsureNotebook(string userId)
        {
            var defaultNotebook = _dbContext.Notebook
                .FirstOrDefault(n => n.UserId == userId && n.Name == "Default");

            if (defaultNotebook == null)
            {
                _dbContext.Notebook.Add(new Notebook
                {
                    Name = "Default",
                    Description = "Varsayılan defteriniz",
                    Color = "bg-secondary",
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId
                });
                _dbContext.SaveChanges();
            }
            var Id = _dbContext.Notebook.FirstOrDefault(x => x.UserId == userId).Id;
            if (Id != null)
                return Id;
            else return -1;
        }
        
        public async Task<Notebook> Get(int id)
        {
           return await _dbContext.Notebook.Include(u=> u.User).
                Include(x => x.Notes).
                ThenInclude(x=> x.Tags).
                FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Notebook> Get(Notebook notebook)
        {
            return await _dbContext.Notebook
                .Include(x => x.Notes)
                .ThenInclude(x => x.User).FirstOrDefaultAsync(x=> x.Id == notebook.Id);
        }

        public async Task<List<Notebook>> ListAll(string userId)
        {
           return await _dbContext.Notebook
                .Include(x=> x.User)
                .ThenInclude(n => n.Notes)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task Update(Notebook notebook)
        {
             _dbContext.Notebook.Update(notebook);
             await _dbContext.SaveChangesAsync();
        }
    }
}
