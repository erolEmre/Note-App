using Azure;
using FluentValidation;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Models.DTOs;
using NoteAppMVCPattern.Models.ViewModel;
using NoteAppMVCPattern.Repo;
using NoteAppMVCPattern.Repo.Notebooks;
using NoteAppMVCPattern.Services.Notebooks;
using NoteAppMVCPattern.Services.Notes;
using NoteAppMVCPattern.Services.Tags;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace NoteAppMVCPattern.Controllers
{
    [Authorize]
    
    public class NoteController : Controller
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // 2. Notları getir (İlişkili Tags koleksiyonları dahil edilmelidir!)
            // _noteService.GetNotes metodu içinde .Include(n => n.Tags) kullandığınızdan emin olun.
            var notes = await _noteService.GetNotes(notebookId,userId, tagIds, sortOrder);

            // 3. Kullanıcının sahip olduğu tüm etiketleri getir (List<Tag> dönmelidir)
            // Bu, dropdown menüde listelemek için kullanılacak tüm mevcut etiketlerdir.
            var tags = await _tagService.GetTags(userId);
            var notebook = await _notebookService.ListAll(userId);

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            int? notebookId = HttpContext.Session.GetInt32("NotebookId");
            if (notebookId == null) return BadRequest("Notebook seçilmemiş.");

            note.NotebookId = notebookId.Value;

            await _noteService.Add(note, userId);

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
                    ModelState.AddModelError(item.ErrorCode,item.ErrorMessage);
                }
                return View(note);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // NameIdentifier --> Id
                await _noteService.Update(note, userId);
                int? notebookId = HttpContext.Session.GetInt32("NotebookId");

                return RedirectToAction("Index", new { notebookId = notebookId.Value });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // NameIdentifier --> Id
            var existedValue = await _noteService.GetNoteById(id, userId);

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _noteService.Delete(id, userId);

            TempData["Message"] = "Not silindi.";
            TempData["MessageType"] = "success";
            int? notebookId = HttpContext.Session.GetInt32("NotebookId");

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


            int? notebookId = HttpContext.Session.GetInt32("NotebookId");

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTag(int noteId, int tagId)
        {
            // ... mantık: noteId ve tagId ile mevcut Tag'i nota ekle
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _tagService.AddToExistingTag(noteId, tagId, userId);
            var note = await _noteService.GetNoteById(noteId, userId);

            var status = TagUpdateStatus.Increment; 
            await _tagService.UpdateTagCount(tagId,status);

            int? notebookId = HttpContext.Session.GetInt32("NotebookId");

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAndAddTag(int noteId, string tagName)
        {
            // ... mantık: yeni Tag'i oluştur, sonra noteId ile nota ekle
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _tagService.CreateAndAdd(noteId, tagName, userId);
            
            // Henüz eklenmedi hata veriyor
            
            var status = TagUpdateStatus.Increment;
            // await kısmına bak,biri bitmeden diğer başladı diye hata fırlatıyor.
            var tag = await _tagService.GetTagByName(tagName);
             _tagService.UpdateTagCount(tag.Id,status);

            int? notebookId = HttpContext.Session.GetInt32("NotebookId");

            return RedirectToAction("Index", new { notebookId = notebookId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDone(int noteId)
        {
            NoteFooterVM note = new NoteFooterVM();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            note.Note = await _noteService.GetNoteById(noteId,userId);
            if (note == null)
            {
                return NotFound(); // veya uygun bir hata mesajı döndür
            }
            
            note.Note.Status = Models.Enums.NoteStatus.Done;
            _tagService.SaveChanges(); 
            
            // View'e gidecek mesaj

            TempData["Message"] = $"Not durumu {note.NoteStatusTurkce} olarak güncellendi";
            TempData["MessageType"] = "info";

            int? notebookId = HttpContext.Session.GetInt32("NotebookId");

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
            
            note.Note.Status = Models.Enums.NoteStatus.Todo;
            _tagService.SaveChanges();

            // View'e gidecek mesaj

            TempData["Message"] = $"Not durumu {note.NoteStatusTurkce} olarak güncellendi.";
            TempData["MessageType"] = "info";


            int? notebookId = HttpContext.Session.GetInt32("NotebookId");

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
            
            note.Note.Status = Models.Enums.NoteStatus.Planned;
            _tagService.SaveChanges();

            // View'e gidecek mesaj

            TempData["Message"] = $"Not durumu {note.NoteStatusTurkce} olarak güncellendi.";
            TempData["MessageType"] = "info";


            int? notebookId = HttpContext.Session.GetInt32("NotebookId");

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
                int? notebookId = HttpContext.Session.GetInt32("NotebookId");

                return RedirectToAction("Index", new { notebookId = notebookId.Value });
            }
            return BadRequest();

        }
        [HttpPost]       
        public async Task<IActionResult> UpdateTitle([FromBody] NoteTitleDTO noteDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var note = await _noteService.GetNoteById(noteDTO.Id, userId);
            note.Title = noteDTO.Title;
            await _noteService.Update(note, userId);

           
            return Ok();
        }
        public async Task<IActionResult> Share(int id)
        {
            return null;
        }             
        
    }
}
