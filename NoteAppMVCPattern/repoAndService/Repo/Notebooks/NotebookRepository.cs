using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Repo.Notebooks
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

        public async Task<Notebook> Get(int id)
        {
           return await _dbContext.Notebook.Include(x=> x.Notes)
                .Include(x => x.User).
                FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Notebook> Get(Notebook notebook)
        {
            return await _dbContext.Notebook
                .Include(x => x.Notes)
                .ThenInclude(x => x.User).FirstOrDefaultAsync(x=> x.Id == notebook.Id);
        }

        public async Task Update(Notebook notebook)
        {
             _dbContext.Notebook.Update(notebook);
             await _dbContext.SaveChangesAsync();
        }
    }
}
