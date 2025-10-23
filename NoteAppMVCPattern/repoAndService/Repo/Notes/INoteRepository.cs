using NoteAppMVCPattern.Models;

namespace NoteAppMVCPattern.Repo.Notes
{
    public interface INoteRepository
    {
        Task<NoteAppMVCPattern.Models.Note> GetByIdAsync(int id, string userId);

        // Bir kullanıcıya ait tüm notları filtreleme ve sıralama seçenekleriyle getirir
        Task<List<Models.Note>> GetAllByUserIdAsync(string userId, List<int> tagIds = null, string sortOrder = null);

        // Kullanıcıya ait benzersiz (distinct) etiketleri getirir      

        Task Add(Models.Note note);
        Task Update(Models.Note note);
        Task Delete(Models.Note note);

    }

}
