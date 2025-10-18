using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Repo
{
    public interface INoteRepository
    {
        Task<Note> GetByIdAsync(int id, string userId);

        // Bir kullanıcıya ait tüm notları filtreleme ve sıralama seçenekleriyle getirir
        Task<List<Note>> GetAllByUserIdAsync(string userId, List<int> tagIds = null, string sortOrder = null);

        // Kullanıcıya ait benzersiz (distinct) etiketleri getirir      

        Task Add(Note note);
        Task Update(Note note);
        Task Delete(Note note);

    }

}
