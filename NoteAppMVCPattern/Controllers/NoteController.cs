using Azure;
using FluentValidation;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Migrations;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Models.ViewModel;
using NoteAppMVCPattern.Services;
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
         

        public NoteController(INoteService noteService, UserManager<AppUser> userManager, IValidator<Note> validator)
        {
            _noteService = noteService;
            _userManager = userManager;
            _validator = validator;
        }
        [Authorize]
        public async Task<IActionResult> Index(string viewMode = "grid", string tag = null, string sortOrder = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Controller artık sadece service'i çağırıyor.
            var notes = await _noteService.GetNotes(userId, tag, sortOrder);
            var tags = await _noteService.GetUserTags(userId);
           
            var vm = new NoteIndexVM
            {
                Notes = notes,
                Tags = tags,
                ViewMode = viewMode,
                CurrentTag = tag,
                SortOrder = sortOrder
            };
            return View(vm);
        }

        public IActionResult Create()
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
                        User = user
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
            await _noteService.Add(note, userId);

            TempData["Message"] = "Not eklendi.";
            TempData["MessageType"] = "success";
            return RedirectToAction("Index");

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
                return RedirectToAction("Index");
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

            return RedirectToAction("Index");

        }
        [HttpGet]
        //public IActionResult FilterByTag(string tag)
        //{

        //    var filteredNotes = _noteService.GetUserTags(userId);

        //    NoteIndexVM noteIndexVM = new NoteIndexVM();
        //    noteIndexVM.Notes = filteredNotes;

        //    return View("Index", noteIndexVM);

        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _noteService.DeleteTag(id,userId);

            TempData["Message"] = "Tag silindi.";
            TempData["MessageType"] = "success";

            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTag(int id, string tag)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _noteService.AddTag(id, tag, userId.ToString());

            TempData["Message"] = "Tag eklendi.";
            TempData["MessageType"] = "success";

           
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Share(int id)
        {
            return null;
        }
        //public class NoteController : Controller
        //{
        //    private readonly AppDBContext _dbContext;
        //    public NoteController(AppDBContext dbContext)
        //    {
        //        _dbContext = dbContext;
        //    }
        //    public IActionResult Index()
        //    {
        //        var notes = _dbContext.Notes.Include(n => n.User).ToList();
        //        return View(notes);
        //    }
        //    public IActionResult Create()
        //    {
        //        ViewBag.UserList = new SelectList(_dbContext.Users, "Id", "Username");
        //        return View();
        //    }
        //    [HttpPost]
        //    public IActionResult Create(Note note)
        //    {
        //        //note.UserId = 1;
        //        if (!ModelState.IsValid)
        //        {
        //            foreach (var key in ModelState.Keys)
        //            {
        //                var errors = ModelState[key].Errors;
        //                foreach (var error in errors)
        //                {
        //                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
        //                }
        //            }
        //            return View(note);
        //        }

        //        _dbContext.Notes.Add(note);
        //        _dbContext.SaveChanges();
        //        TempData["msg"] = "Notunuz başarıyla eklendi";
        //        return RedirectToAction("Index");
        //    }
        //    [HttpPost]
        //    public IActionResult Delete(int id)
        //    {
        //        var note = _dbContext.Notes.FirstOrDefault(x=> x.Id == id);
        //        if (note != null)
        //        {
        //            _dbContext.Notes.Remove(note);
        //            _dbContext.SaveChanges();
        //            TempData["Message"] = $"{note.Title} silindi.";
        //        }
        //        else
        //        {
        //            TempData["Message"] = "Not bulunamadı.";
        //        }

        //        return RedirectToAction("Index");
        //    }
        //    [HttpPost]
        //    public IActionResult Update(Note note)
        //    {
        //        var existednote = _dbContext.Notes.FirstOrDefault(x => x.Id == note.Id);
        //        if (note != null)
        //        {
        //            existednote.Title = note.Title;
        //            existednote.Content = note.Content;
        //        }
        //        TempData["msg"] = "Güncelleme başarılı";
        //        _dbContext.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    public IActionResult Update(int id)
        //    {
        //        var note = _dbContext.Notes.FirstOrDefault(u => u.Id == id);
        //        if (note == null) return NotFound();
        //        _dbContext.SaveChanges();
        //        return View(note);
        //    }
        //    public IActionResult Details(int id)
        //    {
        //        var note = _dbContext.Notes.FirstOrDefault(x=>x.Id == id);
        //        if (note != null)
        //        {
        //            return View(note);
        //        }

        //        return View(note);
        //    }       
        //}
       
        
    }
}
