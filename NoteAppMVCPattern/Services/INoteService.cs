using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Services
{
    public interface INoteService
    {
        public Task<Note> GetNoteById(int id, string userId);
        public Task<List<Note>> GetNotes(string userId, string tag = null, string sortOrder = null);
        public Task<List<string>> GetUserTags(string userId);
        public Task Add(Note note, string userId);
        public Task Update(Note updatedNote, string userId);
        public Task Delete(int id, string userId);
        public Task AddTag(int noteId, string tag, string userId);
        public Task DeleteTag(int noteId, string userId);
    }
}
