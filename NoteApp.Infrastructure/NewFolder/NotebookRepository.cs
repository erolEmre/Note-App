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
           
            // 1. Kullanıcıya ait, adı "Default" olan bir defter var mı? (Normal akış)
            var userNotebook = await _dbContext.Notebook
                .FirstOrDefaultAsync(n => n.UserId == userId && n.Name == "Default");

            if (userNotebook != null)
            {
                return userNotebook.Id; // Kullanıcının kendi defterini döndür.
            }

            // 2. ID=1 olan anonim defter boşta mı? (UserId hala NULL mı?)
            var anonymousNotebook = await _dbContext.Notebook
                .FirstOrDefaultAsync(n => n.Id == 1 && n.UserId == null);

            if (anonymousNotebook != null)
            {
                // 3. Anonim defteri kullanıcıya ata (CLAIM et)
                // Bu adım, eski notları kullanan ilk gerçek kullanıcıya ID=1'i bağlar.
                anonymousNotebook.UserId = userId;

                // Bu defterin eski notları da bu kullanıcıya ait olduğu için tutarlı olur.
                await _dbContext.SaveChangesAsync();

                return anonymousNotebook.Id; // ID=1'i döndür.
            }

            // 4. (En kötü senaryo) Ne kullanıcının kendi defteri var, ne de ID=1 boşta. 
            // Bu, ID=1'in başka bir kullanıcı tarafından claim edildiği anlamına gelir.
            // Bu durumda yeni bir defter oluşturulmalıdır (sizin ID=2'niz buydu).
            var newNotebook = new Notebook
            {
                Name = "Default",
                Description = "Varsayılan defteriniz",
                Color = "bg-secondary",
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _dbContext.Notebook.Add(newNotebook);
            await _dbContext.SaveChangesAsync();
            return newNotebook.Id;
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
