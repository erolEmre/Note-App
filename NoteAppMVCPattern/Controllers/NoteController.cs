using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteApp.Application.Models.DTOs;
using NoteApp.WebUI.Models.ViewModel;
using NoteApp.Application.Services.Notebooks;
using NoteApp.Application.Services.Notes;
using NoteApp.Application.Services.Tags;
using System.Security.Claims;
using NoteApp.Core.Entities;
using NoteApp.Application.Services.User;

namespace NoteApp.WebUI.Controllers
{
    [Authorize]
    
    public class NoteController : BaseController
    {
        private readonly INoteService _noteService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<Note> _validator;
        private readonly ITagService _tagService;
        private readonly INotebookService _notebookService;
        public NoteController(ITagService tagService,INoteService noteService, 
            UserManager<AppUser> userManager, IValidator<Note> validator,INotebookService notebookService)
        {
            _noteService = noteService;
            _userManager = userManager;
            _validator = validator;
            _tagService = tagService;
            _notebookService = notebookService;
        }
        [Authorize]
        public async Task<IActionResult> Index(int notebookId,string viewMode = "grid", string sortOrder = null, 
            List<int> tagIds = null)
        {
            HttpContext.Session.SetInt32("NotebookId", notebookId);
            // 1. Kullanıcı ID'sini al
            
            // 2. Notları getir (İlişkili Tags koleksiyonları dahil edilmelidir!)
            // _noteService.GetNotes metodu içinde .Include(n => n.Tags) kullandığınızdan emin olun.
            var notes = await _noteService.GetNotes(notebookId,UserId, tagIds, sortOrder);

            // 3. Kullanıcının sahip olduğu tüm etiketleri getir (List<Tag> dönmelidir)
            // Bu, dropdown menüde listelemek için kullanılacak tüm mevcut etiketlerdir.
            var tags = await _tagService.GetTags(UserId); // Burası hatalı işlem yapıyor
            var notebook = await _notebookService.ListAll(UserId);

            // 4. ViewModel oluştur
            var vm = new NoteIndexVM
                {
                    Notes = notes,
                    Tags = tags,
                    SelectedTagIds = tagIds ?? new List<int>(),
                    ViewMode = viewMode,
                    SortOrder = sortOrder,
                    Notebooks = notebook,
                    NotebookID = notebookId
                };
                
                vm.CurrentTag = vm.SelectedTagIds.Any()
                ? await _tagService.GetTag(vm.SelectedTagIds.First())
                : null;


            return View(vm);
        }

        public IActionResult Create(int notebookId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
               
                if (userId != null)
                {
                    var user = _userManager.Users
                        .Include(u => u.Notes)
                        .FirstOrDefault(u => u.Id == Guid.Parse(userId).ToString());

                    var note = new Note
                    {
                        User = user,
                        NotebookId = notebookId
                    };                   
                    return View(note);
                }
            }
            return View(new Note()); // fallback
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Note note)
        {
            

            int? notebookId = NotebookId;
            if (notebookId == null) return BadRequest("Notebook seçilmemiş.");

            note.NotebookId = notebookId.Value;

            await _noteService.Add(note, UserId);

            TempData["Message"] = "Not eklendi.";
            TempData["MessageType"] = "success";
            return RedirectToAction("Index",new { notebookId = notebookId.Value });

        }
        [HttpGet]
        // [ValidateAntiForgeryToken] ! Bu varken çalışmıyor..
        public IActionResult NoteDetails(int noteId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null)
                {
                    var user = _userManager.Users
                        .Include(u => u.Notes)
                        .ThenInclude(t => t.Tags)
                        .FirstOrDefault(u => u.Id == Guid.Parse(userId).ToString());
                    
                        foreach (var note in user.Notes)
                        {
                            if (note.Id == noteId)
                            {
                                return PartialView("_noteModalPartial", note);
                            }
                        }
                    
                }
            }
                    return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Note note)
        {
            var result = await _validator.ValidateAsync(note);

            if (!result.IsValid)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.PropertyName,item.ErrorMessage);
                }
                return View(note);
            }
            else
            {
                 // NameIdentifier --> Id
                await _noteService.Update(note, UserId);
                int? notebookId = NotebookId;

                return RedirectToAction("Index", new { notebookId = notebookId.Value });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

             // NameIdentifier --> Id
            var existedValue = await _noteService.GetNoteById(id, UserId);

            if (existedValue == null)
            {
                TempData["Message"] = "Aradık,aradık ama bulamadık.";
                TempData["MessageType"] = "info";
            }
            return View(existedValue);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            
            await _noteService.Delete(id, UserId);

            TempData["Message"] = "Not silindi.";
            TempData["MessageType"] = "success";
            int? notebookId = NotebookId;

            return RedirectToAction("Index", new { notebookId = notebookId.Value });

        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTag(int noteId,string userId,int tagId)
        {
            var _userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = _userId;


            var status = TagUpdateStatus.Decrement;
            await _tagService.UpdateTagCount(tagId, status);
            await _tagService.DeleteTag(noteId,userId,tagId);
            
            TempData["Message"] = "Tag silindi.";
            TempData["MessageType"] = "success";


            int? notebookId = NotebookId;

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTag(int noteId, int tagId)
        {
            // ... mantık: noteId ve tagId ile mevcut Tag'i nota ekle
            
            await _tagService.AddToExistingTag(noteId, tagId, UserId);
            var note = await _noteService.GetNoteById(noteId, UserId);

            var status = TagUpdateStatus.Increment; 
            await _tagService.UpdateTagCount(tagId,status);

            int? notebookId = NotebookId;

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAndAddTag(int noteId, string tagName)
        {
            // ... mantık: yeni Tag'i oluştur, sonra noteId ile nota ekle
            
            await _tagService.CreateAndAdd(noteId, tagName, UserId);
            
            // Henüz eklenmedi hata veriyor
            
            var status = TagUpdateStatus.Increment;
            // await kısmına bak,biri bitmeden diğer başladı diye hata fırlatıyor.
            var tag = await _tagService.GetTagByName(tagName);
             _tagService.UpdateTagCount(tag.Id,status);

            int? notebookId = NotebookId;

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDone(int noteId)
        {
            NoteFooterVM note = new NoteFooterVM();
            
            
            note.Note = await _noteService.GetNoteById(noteId, UserId);
            if (note == null)
            {
                return NotFound(); // veya uygun bir hata mesajı döndür
            }
            
            note.Note.Status = Core.Entities.Enums.NoteStatus.Done;
            _tagService.SaveChanges(); 
            
            // View'e gidecek mesaj

            TempData["Message"] = $"Not durumu {note.NoteStatusTurkce} olarak güncellendi";
            TempData["MessageType"] = "info";

            int? notebookId = NotebookId;

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsToDo(int noteId)
        {
            NoteFooterVM note = new NoteFooterVM();
            note.Note = await _noteService.GetNoteById(noteId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (note == null)
            {
                return NotFound(); // veya uygun bir hata mesajı döndür
            }
            
            note.Note.Status = Core.Entities.Enums.NoteStatus.Todo;
            _tagService.SaveChanges();

            // View'e gidecek mesaj

            TempData["Message"] = $"Not durumu {note.NoteStatusTurkce} olarak güncellendi.";
            TempData["MessageType"] = "info";


            int? notebookId = NotebookId;

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPlanned(int noteId,DateTime dateTime)
        {
            NoteFooterVM note = new NoteFooterVM();
            note.Note = await _noteService.GetNoteById(noteId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (note == null)
            {
                return NotFound(); // veya uygun bir hata mesajı döndür
            }
            
            note.Note.Status = Core.Entities.Enums.NoteStatus.Planned;
            _tagService.SaveChanges();

            // View'e gidecek mesaj

            TempData["Message"] = $"Not durumu {note.NoteStatusTurkce} olarak güncellendi.";
            TempData["MessageType"] = "info";


            int? notebookId = NotebookId;

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }
        [HttpGet]
        public async Task<IActionResult> TagColorPage(int tagId)
        {
            Tag tag = await _tagService.GetTag(tagId);
            if(tag != null)
            {
                return PartialView("_tagColorUpdate",tag);
            }
            else 
            { 
                return NotFound(); 

            }         
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> tagColorUpdate(int tagId,string tagColor)
        {
            Tag tag = await _tagService.GetTag(tagId);
            if (tag != null)
            {
                tag.TagColor = tagColor;
                _tagService.SaveChanges();
                int? notebookId = NotebookId;

                return RedirectToAction("Index", new { notebookId = notebookId.Value });
            }
            return BadRequest();

        }
        [HttpPost]       
        public async Task<IActionResult> UpdateTitle([FromBody] NoteTitleDTO noteDTO)
        {
             
            var note = await _noteService.GetNoteById(noteDTO.Id, UserId);
            note.Title = noteDTO.Title;
            await _noteService.Update(note, UserId);

           
            return Ok();
        }
        public async Task<IActionResult> Share(int id)
        {
            return null;
        }             
        
    }
}
