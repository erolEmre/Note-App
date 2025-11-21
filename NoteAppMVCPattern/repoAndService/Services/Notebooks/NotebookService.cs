using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Repo.Notebooks;

namespace NoteAppMVCPattern.Services.Notebooks
{
    public class NotebookService : INotebookService
    {
        private readonly INotebookRepository _notebookRepository;
        public NotebookService(INotebookRepository notebookRepository) 
        { 
                _notebookRepository = notebookRepository;
        }
        public async Task Add(Notebook notebook)
        {
            if (notebook != null)
            {
               await _notebookRepository.Add(notebook);
            }
        }

        public async Task Delete(Notebook notebook)
        {
            if (notebook != null)
            {
                await _notebookRepository.Delete(notebook);
            }
        }

        public async Task<int> EnsureNotebook(string userId)
        {
            var item = await _notebookRepository.EnsureNotebook(userId);
            if (item == -1) return -1;
            return item;
        }

        public async Task<Notebook> Get(int id)
        {
           return await _notebookRepository.Get(id);
        }

        public async Task<Notebook> Get(Notebook notebook)
        {
            if (notebook != null)
            {
                return await _notebookRepository.Get(notebook);
            }
            else return null;
        }

        public async Task<List<Notebook>> ListAll(string userId)
        {
            var list = await _notebookRepository.ListAll(userId);
            if (list != null)
            {
                return list;
            }
            else 
            { 
                return new List<Notebook>(); 
            }
        }

        public async Task Update(Notebook notebook)
        {
            var oldNotebook = await _notebookRepository.Get(notebook.Id);
            if (oldNotebook != null)
            {
                oldNotebook.Name = notebook.Name;
                oldNotebook.Description = notebook.Description;
                if(notebook.Color == null)
                {
                    notebook.Color = "bg-primary";
                }
                oldNotebook.Color = notebook.Color;
                oldNotebook.UpdatedAt = DateTime.UtcNow;

                await _notebookRepository.Update(oldNotebook);
            }
        }
    }
}
