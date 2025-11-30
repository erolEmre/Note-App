using NoteApp.Core.Entities;

namespace NoteApp.Application.Repo.Notes
{
    public interface INoteRepository
    {
        Task<Note?> GetByIdAsync(int id,string userId);
        Task<List<Note>> GetAllAsync();
        Task<List<Note>> GetByNotebookIdAsync(int notebookId);

        Task Add(Note note);
        Task Update(Note note);
        Task Delete(Note note);

    }

}
