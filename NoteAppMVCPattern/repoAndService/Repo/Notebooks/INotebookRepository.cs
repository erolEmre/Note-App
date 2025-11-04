using NoteAppMVCPattern.Models;
namespace NoteAppMVCPattern.Repo.Notebooks
{
    public interface INotebookRepository
    {
        public Task<Notebook> Get(int id);
        public Task<Notebook> Get(Notebook notebook);
        public Task Update(Notebook notebook);
        public Task Delete(Notebook notebook);
        public Task Add(Notebook notebook);
        public Task<List<Notebook>> ListAll(string userId);
        public Task<int> EnsureNotebook(string userId);
    }
}
