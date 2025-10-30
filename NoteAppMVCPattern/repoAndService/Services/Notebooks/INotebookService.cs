using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Services.Notebooks
{
    public interface INotebookService 
    {
        public Task<Notebook> Get(int id);
        public Task<Notebook> Get(Notebook notebook);
        public Task Update(Notebook notebook);
        public Task Delete(Notebook notebook);
        public Task Add(Notebook notebook);
        public Task<List<Notebook>> ListAll();
    }
}
