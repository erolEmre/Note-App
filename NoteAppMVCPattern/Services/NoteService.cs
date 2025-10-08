using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Repo;
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

            // İçerikten etiket çıkarma mantığı
            if (!string.IsNullOrEmpty(note.Content))
            {
                var match = Regex.Match(note.Content, @"#(\w+)");
                if (match.Success)
                {
                    note.Tag = match.Groups[1].Value;
                }
            }
            // --- İŞ MANTIĞI BİTTİ ---

            // Hazırlanan nesneyi repository'ye gönderiyoruz.
            await _noteRepository.Add(note);
        }

        public async Task AddTag(int noteId, string tag, string userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId, userId);

            // 2. Not varsa ve kullanıcıya aitse, etiketini güncelle.
            if (note != null)
            {
                note.Tag = tag;
                note.updatedDate = DateTime.UtcNow; // Güncelleme tarihini de değiştirelim
                await _noteRepository.Update(note);
            }
        }

        public async Task Delete(int id, string userId)
        {
            var noteToDelete = await _noteRepository.GetByIdAsync(id, userId);
            if (noteToDelete != null)
            {
                await _noteRepository.Delete(noteToDelete);
            }
        }

        public async Task DeleteTag(int noteId, string userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId, userId);

            // 2. Not varsa ve kullanıcıya aitse, etiketini null yap.
            if (note != null)
            {
                note.Tag = null;
                note.updatedDate = DateTime.UtcNow;
                await _noteRepository.Update(note);
            }
        }
        public async Task<Note> GetNoteById(int id, string userId)
        {
            return await _noteRepository.GetByIdAsync(id, userId);
        }

        public async Task<List<Note>> GetNotes(string userId, string tag = null, string sortOrder = null)
        {
            return await _noteRepository.GetAllByUserIdAsync(userId, tag, sortOrder);
        }

        public async Task<List<string>> GetUserTags(string userId)
        {
            return await _noteRepository.GetTags(userId);
        }

        public async Task Update(Note updatedNote, string userId)
        {
            var existingNote = await _noteRepository.GetByIdAsync(updatedNote.Id, userId);
            if (existingNote == null)
            {
                throw new Exception();
            }

            // --- GÜNCELLEME İŞ MANTIĞI ---
            existingNote.Title = updatedNote.Title;
            existingNote.Content = updatedNote.Content;
            existingNote.updatedDate = DateTime.UtcNow;

            var match = Regex.Match(updatedNote.Content, @"#(\w+)");
            existingNote.Tag = match.Success ? match.Groups[1].Value : null;
            // --- BİTTİ ---

            await _noteRepository.Update(existingNote);
        }
      
    }
}
