using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Repo;

namespace NoteAppMVCPattern.Services
{
    public class TagService : ITagService
    {

        private readonly ITagRepository _tagRepository;
        private readonly INoteRepository _noteRepository;
        public TagService(ITagRepository tagRepository, INoteRepository noteRepository)
        {
            _tagRepository = tagRepository;
            _noteRepository = noteRepository;
        }
        public async Task UpdateTagCount(int TagId,TagUpdateStatus status)
        {
            await _tagRepository.UpdateTagCountAsync(TagId,status);
        }

        public async Task<Tag> GetTagByName(string tagName)
        {
            var tag = await _tagRepository.GetTagByName(tagName);
            if (tag == null)
                throw new Exception("Tag bulunamadı.");
            return tag;
        }
        public async Task AddToExistingTag(int noteId, int tagId, string userId)
        {
            // 1. Notu ve ilişkili Tag'leri Include ile getiriyoruz (Repo'nun doğru çalıştığı varsayılır)
            var note = await _noteRepository.GetByIdAsync(noteId, userId);
            if (note == null)
                throw new Exception("Note not found.");

            // 2. Eklenecek Tag'i bul
            var tagToAdd = await _tagRepository.GetTags(userId); // Repoya yeni metot eklemeyi gerektirir
            if (tagToAdd == null)
                throw new Exception("Tag not found.");

            // 3. Kontrol: Tag zaten notta ekli mi?
            if (note.Tags.Any(t => t.Id == tagId))
            {
                // Tag zaten ekli, işlem yapmaya gerek yok.
                return;
            }

            // 4. Tag'i koleksiyona ekle (EF Core ara tabloyu otomatik güncelleyecektir)
            foreach (var tag in tagToAdd)
            {
                if (tag.Id == tagId && !note.Tags.Contains(tag))
                {
                    note.Tags.Add(tag);
                }
            }

            note.updatedDate = DateTime.UtcNow;
            await _noteRepository.Update(note);
        }
        public async Task DeleteTag(int noteId, string userId, int tagId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId, userId);

            if (note == null)
                throw new Exception("Note not found.");

            // 2. Notun mevcut Tag koleksiyonu içinde, silmek istediğimiz Tag'i bul.
            // note.Tags koleksiyonu zaten veritabanından yüklendiği için bu aramayı yapabiliriz.
            var tagToRemove = note.Tags.FirstOrDefault(tag => tag.Id == tagId);

            if (tagToRemove == null)
            {
                // Tag bu notta zaten yok veya bulunamadı. Hata fırlatmak yerine işlem yapmadan çıkılabilir.
                return;
            }

            // 3. ✨ KRİTİK ADIM: Koleksiyondan Tag nesnesini kaldır.
            // EF Core, bu Tag nesnesinin kendisini değil, sadece NoteTag tablosundaki ilişkiyi siler.
            note.Tags.Remove(tagToRemove);

            // 4. Güncelleme tarihini ayarla
            note.updatedDate = DateTime.UtcNow;

            // 5. Değişiklikleri veritabanına kaydet
            await _noteRepository.Update(note);
        }
        public async Task<List<Tag>> GetTags(string userId)
        {
            return await _tagRepository.GetTags(userId);
        }

        public async Task<List<Tag>> GetUserTags(string userId)
        {
            return await _tagRepository.GetTags(userId);
        }
        public async Task CreateAndAdd(int noteId, string tagName, string userId)
        {
            // 1. Notu getir
            var note = await _noteRepository.GetByIdAsync(noteId, userId);
            if (note == null)
                throw new Exception("Note not found.");

            // Tag adını temizle (Regex vb.) ve normalize et
            string normalizedTagName = tagName.Trim().ToLowerInvariant();

            // 2. Bu isimde Tag var mı kontrol et
            var existingTags = await _tagRepository.GetTags(userId);
            var existingTag = existingTags.FirstOrDefault(t => t.TagName.ToLowerInvariant() == normalizedTagName);

            if (existingTag == null)
            {
                // 3. Tag yoksa oluştur
                existingTag = new Tag
                {
                    TagName = tagName.Trim(),
                    TagColor = "bg-warning" // Varsayılan bir renk atayın
                };
                // Not: EF Core, Tag'i note.Tags'e eklediğinizde bu yeni Tag'i de veritabanına ekler.
            }
            else if (note.Tags.Any(t => t.Id == existingTag.Id))
            {
                // Tag zaten ekli, çık
                return;
            }

            // 4. Tag'i nota ekle
            note.Tags.Add(existingTag);

            note.updatedDate = DateTime.UtcNow;
            await _noteRepository.Update(note);
        }

        public async Task<Tag> GetTag(int tagId)
        {
            if (tagId < 0)
            {
                throw new Exception();
            }
            else return _tagRepository.GetTag(tagId);
        }
        public void SaveChanges()
        {
            _tagRepository.SaveChanges();
        }
       
    }
}
