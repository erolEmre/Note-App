using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Migrations;
using NoteAppMVCPattern.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;
using LinqKit;
using NoteAppMVCPattern.Models.ViewModel;
using FluentValidation;

namespace NoteAppMVCPattern.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<Note> _validator;

        public NoteController(AppDBContext dbContext, UserManager<AppUser> userManager, IValidator<Note> validator)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _validator = validator; 
        }
        [Authorize]
        public async Task<IActionResult> Index(string viewMode = "grid", string tag = null, string sortOrder = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Notları filtrele
            var notesQuery = _dbContext.Notes.Where(n => n.UserId == userId);

            if (!string.IsNullOrEmpty(tag))
                notesQuery = notesQuery.Where(n => n.Tag == tag);

            notesQuery = sortOrder switch
            {
                "date_asc" => notesQuery.OrderBy(n => n.updatedDate),
                "date_desc" => notesQuery.OrderByDescending(n => n.updatedDate),
                _ => notesQuery.OrderByDescending(n => n.updatedDate)
            };

            var notes = await notesQuery.ToListAsync();

            var tags = await _dbContext.Notes
                .Where(n => n.UserId == userId && n.Tag != null)
                .Select(n => n.Tag)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();

            // ViewModel oluştur
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
                    var user = _dbContext.Users
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
        public async Task<IActionResult> Create(Note note)
        {
            note.CreateDate = DateTime.UtcNow;
            note.updatedDate = DateTime.UtcNow;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            note.UserId = userId;

            var match = Regex.Match(note.Content, @"#(\w+)");
            if (note.Content != null && match.Success)
            {
                note.Tag = match.Groups[1].Value;
            }

            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync();

            TempData["Message"] = "Note has been added.";
            TempData["MessageType"] = "success";
            return RedirectToAction("Index");

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(Note note)
        {
            var result = _validator.Validate(note);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(e => e.ErrorMessage));
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // NameIdentifier --> Id
                var existedValue = await _dbContext.Notes.FirstOrDefaultAsync
                    (x => x.Id == note.Id && x.UserId == userId);
                if (existedValue != null)
                {
                    existedValue.Title = note.Title;
                    existedValue.Content = note.Content;

                    if (existedValue.Content != null)
                    {
                        var match = Regex.Match(note.Content, @"#(\w+)");
                        if (match.Success)
                        {
                            existedValue.Tag = match.Groups[1].Value;
                        }
                    }
                    else existedValue.Content = "";
                    existedValue.updatedDate = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // NameIdentifier --> Id
            var existedValue = await _dbContext.Notes.FirstOrDefaultAsync
                (x => x.Id == id && x.UserId == userId);

            if (existedValue == null)
            {
                TempData["Message"] = "We searched everywhere, but couldn't find anything.";
                TempData["MessageType"] = "info";
            }
            return View(existedValue);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // NameIdentifier --> Id
            var existedValue = await _dbContext.Notes.FirstOrDefaultAsync
                (x => x.Id == id && x.UserId == userId);

            if (existedValue == null)
            {
                TempData["Message"] = "We searched everywhere, but couldn't find anything.";
                TempData["MessageType"] = "info";
            }

            _dbContext.Notes.Remove(existedValue);
            await _dbContext.SaveChangesAsync();

            TempData["Message"] = "Note has been deleted.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult FilterByTag(string tag)
        {
            var filteredNotes = _dbContext.Notes
                .Where(n => n.Tag == tag)
                .ToList();

            NoteIndexVM noteIndexVM = new NoteIndexVM();
            noteIndexVM.Notes = filteredNotes;

            return View("Index", noteIndexVM);

        }

        public async Task<IActionResult> DeleteTag(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (note != null) note.Tag = null;

            TempData["Message"] = "Tag has been deleted.";
            TempData["MessageType"] = "success";

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddTag(int id, string tag)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (note != null) note.Tag = tag;

            TempData["Message"] = "Note has been added.";
            TempData["MessageType"] = "success";

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
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
