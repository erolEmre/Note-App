using NoteApp.Application.Repo.Notebooks;
using NoteApp.Core.Entities;
using NoteApp.Application.Repo.Notes;
using NoteApp.Application.Repo.Tags;
using System.Text.RegularExpressions;

namespace NoteApp.Application.Services.Notes
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly ITagRepository _tagRepository;
        public NoteService(INoteRepository noteRepository, ITagRepository tagRepository)
        {
            _noteRepository = noteRepository;
            _tagRepository = tagRepository;
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
                    var existingTags = await _tagRepository.GetTags(userId);
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
                        tag.TagUsageCount++;
                    }

                    note.Tags = matchedTags.Concat(newTags).ToList();
                }
            }
            await _noteRepository.Add(note);
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

        public Task<List<Note>> GetAllNotes()
        {
            return _noteRepository.GetAllAsync();
        }

        public async Task<Note> GetNoteById(int id, string userId)
        {
            return await _noteRepository.GetByIdAsync(id, userId);
        }

        //public async Task<List<Note>> GetNotes(int notebookId,string userId, List<int> tagIds, string sortOrder = null)
        //{
        //    return await _noteRepository.GetAllByUserIdAsync(notebookId,userId, tagIds, sortOrder);           
        //}

        public async Task<List<Note>> GetNotes(int notebookId,string userId,List<int>? tagIds,string? sortOrder)
        {
            // 1) Repository’den ham veriyi al
            var notes = await _noteRepository.GetByNotebookIdAsync(notebookId);

            // 2) User filter (business logic)
            notes = notes.Where(n => n.UserId == userId).ToList();

            // 3) Tag filter
            if (tagIds != null && tagIds.Any())
                notes = notes.Where(n => n.Tags.Any(t => tagIds.Contains(t.Id))).ToList();

            // 4) Sort
            notes = sortOrder switch
            {
                "date_asc" => notes.OrderBy(n => n.updatedDate).ToList(),
                "date_desc" => notes.OrderByDescending(n => n.updatedDate).ToList(),
                "title" => notes.OrderBy(n => n.Title).ToList(),
                _ => notes.OrderByDescending(n => n.updatedDate).ToList()
            };

            return notes;
        }


        public async Task Update(Note updatedNote, string userId)
        {
            var existingNote = await _noteRepository.GetByIdAsync(updatedNote.Id, userId);
            if (existingNote == null)
                throw new Exception("Note not found.");

            existingNote.Title = updatedNote.Title;
            existingNote.Content = updatedNote.Content;
            existingNote.updatedDate = DateTime.UtcNow;

            // İçerikten tag'leri çek
            var matches = Regex.Matches(updatedNote.Content ?? "", @"#(\w+)"); 
            // Güncelleme sırasında  "#" başlayanları çek
            var tagNames = matches.Select(m => m.Groups[1].Value).ToList(); 
            // # ile başlayan değerleri liste şeklinde tagNames'e at

            var existingTags = await _tagRepository.GetTags(userId); // Kullanıcıya ait tüm tagleri çek
            // hatta buranın notebook içindeki tüm tagler olarak düzeltilmesi lazım
            var matchedTags = existingTags.Where(t => tagNames.Contains(t.TagName)).ToList();
            // Existing içinde hali hazırda üretilmiş tag varsa bul liste olarak at.

            var myNoteTags = await _tagRepository.GetTagsByNote(existingNote.Id);

            var newTags = tagNames
                .Except(matchedTags.Select(t => t.TagName))
                .Select(t => new Tag { TagName = t })
                .ToList();
            // NewTags'e sadece tagNames içindeki verileri çek diğerleri dışarıda tut
            
            foreach (var tag in newTags)
            {
                tag.TagColor = "bg-primary";
                tag.TagUsageCount++;
            }

            existingNote.Tags = myNoteTags.Concat(newTags).ToList(); //mynoteTags yerine matchedTags yazıyordu
            // Uyuşan-match olan tagler ile yenileri birleştir

            await _noteRepository.Update(existingNote);
        }
        
    }
}
