using NoteApp.Core.Entities;

namespace NoteApp.Application.Repo.Notes
{
    public interface INoteRepository
    {
        Task<Note> GetByIdAsync(int id, string userId);

        // Bir kullanıcıya ait tüm notları filtreleme ve sıralama seçenekleriyle getirir
        Task<List<Note>> GetAllByUserIdAsync(int notebookId, string userId, List<int> tagIds = null, string sortOrder = null);

        // Kullanıcıya ait benzersiz (distinct) etiketleri getirir      

        Task Add(Note note);
        Task Update(Note note);
        Task Delete(Note note);

    }

}
