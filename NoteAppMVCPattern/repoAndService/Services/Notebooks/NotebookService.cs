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

        public async Task<List<Notebook>> ListAll()
        {
            var list = await _notebookRepository.ListAll();
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
                oldNotebook.Color = notebook.Color;
                oldNotebook.UpdatedAt = DateTime.UtcNow;

                await _notebookRepository.Update(oldNotebook);
            }
        }
    }
}
