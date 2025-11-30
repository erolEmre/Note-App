using NoteApp.Core.Entities;

namespace NoteApp.Application.Services.Notes
{
    public interface INoteService
    {
        public Task<Note> GetNoteById(int id, string userId);
        public Task<List<Note>> GetNotes(int notebookId,string userId, List<int> tagIds, string sortOrder = null);
        public Task<List<Note>> GetAllNotes();      
        public Task Add(Note note, string userId);
        public Task Update(Note updatedNote, string userId);
        public Task Delete(int id, string userId);
        
    }
}
