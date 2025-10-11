using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Repo;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace NoteAppMVCPattern.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }
        public async Task Add(Note note, string userId)
        {
            note.UserId = userId;
            note.CreateDate = DateTime.UtcNow;
            note.updatedDate = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(note.Content))
            {
                var matches = Regex.Matches(note.Content, @"#(\w+)");
                if (matches.Count > 0)
                {
                    var tagNames = matches.Select(m => m.Groups[1].Value).ToList();

                    // veritabanındaki mevcut tag'leri al
                    var existingTags = await _noteRepository.GetTags(userId);
                    var matchedTags = existingTags
                        .Where(t => tagNames.Contains(t.TagName))
                        .ToList();

                    // olmayan tag’leri oluştur
                    var newTags = tagNames
                        .Except(matchedTags.Select(t => t.TagName))
                        .Select(t => new Tag { TagName = t })
                        .ToList();

                    foreach (var tag in newTags)
                    {
                        tag.TagColor = "bg-secondary";
                    }

                    note.Tags = matchedTags.Concat(newTags).ToList();
                }
            }

            await _noteRepository.Add(note);
        }

        public async Task AddToExistingTag(int noteId, int tagId, string userId)
        {
            // 1. Notu ve ilişkili Tag'leri Include ile getiriyoruz (Repo'nun doğru çalıştığı varsayılır)
            var note = await _noteRepository.GetByIdAsync(noteId, userId);
            if (note == null)
                throw new Exception("Note not found.");

            // 2. Eklenecek Tag'i bul
            var tagToAdd = await _noteRepository.GetTags(userId); // Repoya yeni metot eklemeyi gerektirir
            if (tagToAdd == null)
                throw new Exception("Tag not found.");

            //// 3. Kontrol: Tag zaten notta ekli mi?
            //if (note.Tags.Any(t => t.Id == tagId))
            //{
            //    // Tag zaten ekli, işlem yapmaya gerek yok.
            //    return;
            //}

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

        // Reponuzda GetTagById metodu yoksa, GetTags(userId) listesi üzerinden de bulabilirsiniz:
        /*
        var allTags = await _noteRepository.GetTags(userId);
        var tagToAdd = allTags.FirstOrDefault(t => t.Id == tagId);
        */


        public async Task Delete(int id, string userId)
        {
            var noteToDelete = await _noteRepository.GetByIdAsync(id, userId);
            if (noteToDelete != null)
            {
                await _noteRepository.Delete(noteToDelete);
            }
        }

        public async Task DeleteTag(int noteId, string userId,int tagId)
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

        public async Task<Note> GetNoteById(int id, string userId)
        {
            return await _noteRepository.GetByIdAsync(id, userId);
        }

        public async Task<List<Note>> GetNotes(string userId, List<int> tagIds, string sortOrder = null)
        {
            return await _noteRepository.GetAllByUserIdAsync(userId, tagIds, sortOrder);           
        }


        public async Task<List<Tag>> GetTags(string userId)
        {
            return await _noteRepository.GetTags(userId);
        }

        public async Task<List<Tag>> GetUserTags(string userId)
        {
            return await _noteRepository.GetTags(userId);
        }

        public async Task Update(Note updatedNote, string userId)
        {
            var existingNote = await _noteRepository.GetByIdAsync(updatedNote.Id, userId);
            if (existingNote == null)
                throw new Exception("Note not found.");

            existingNote.Title = updatedNote.Title;
            existingNote.Content = updatedNote.Content;
            existingNote.updatedDate = DateTime.UtcNow;

            // İçerikten tag’leri çek
            var matches = Regex.Matches(updatedNote.Content ?? "", @"#(\w+)");
            var tagNames = matches.Select(m => m.Groups[1].Value).ToList();

            var existingTags = await _noteRepository.GetTags(userId);
            var matchedTags = existingTags.Where(t => tagNames.Contains(t.TagName)).ToList();

            var newTags = tagNames
                .Except(matchedTags.Select(t => t.TagName))
                .Select(t => new Tag { TagName = t })
                .ToList();

            foreach (var tag in newTags)
            {
                tag.TagColor = "bg-primary";
            }

            existingNote.Tags = matchedTags.Concat(newTags).ToList();

            await _noteRepository.Update(existingNote);
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
            var existingTags = await _noteRepository.GetTags(userId);
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

        //async Task<List<Tag>> INoteService.GetTags(string userId)
        //{
        //    var tags = await _noteRepository.GetTags(userId);
        //    return tags.Select(t => t.Id).ToList();
        //}
    }
}
