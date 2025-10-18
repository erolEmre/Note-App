using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Services
{
    public interface INoteService
    {
        public Task<Note> GetNoteById(int id, string userId);
        public Task<List<Note>> GetNotes(string userId, List<int> tagIds, string sortOrder = null);     
        
       
        public Task Add(Note note, string userId);
        public Task Update(Note updatedNote, string userId);
        public Task Delete(int id, string userId);
        
    }
}
