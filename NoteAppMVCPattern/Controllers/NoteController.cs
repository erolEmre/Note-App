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

namespace NoteAppMVCPattern.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public NoteController(AppDBContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
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
            if (tag != null)
            {
            
            }
            return View(vm);
        }

        //public async Task<IActionResult> Index(string tag, string sortOrder)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // 1. Başlangıçta tüm kullanıcı notlarını seç
        //    var predicate = PredicateBuilder.New<Note>(n => n.UserId == userId);

        //    // 2. Tag filtresi varsa ekle
        //    if (!string.IsNullOrEmpty(tag))
        //        predicate = predicate.And(n => n.Tag == tag);

        //    // 3. Query oluştur
        //    var notesQuery = _dbContext.Notes.AsExpandable().Where(predicate);

        //    // 4. Sıralama uygula
        //    notesQuery = sortOrder switch
        //    {
        //        "date_asc" => notesQuery.OrderBy(n => n.updatedDate),
        //        "date_desc" => notesQuery.OrderByDescending(n => n.updatedDate),
        //        _ => notesQuery.OrderByDescending(n => n.updatedDate)
        //    };

        //    // 5. Veriyi çek
        //    var notes = await notesQuery.ToListAsync();

        //    // 6. Tag listesi oluştur
        //    var tags = await _dbContext.Notes
        //        .Where(n => n.UserId == userId && n.Tag != null)
        //        .Select(n => n.Tag)
        //        .Distinct()
        //        .OrderBy(name => name)
        //        .ToListAsync();

        //    // 7. ViewBag ile View'a aktar
        //    ViewBag.Tags = tags;
        //    ViewBag.CurrentTag = tag;
        //    ViewBag.CurrentSort = sortOrder;

        //    var vm = new NoteIndexVM
        //    {
        //        Notes = notes,
        //        Tags = tags,
        //        CurrentTag = tag,
        //        SortOrder = sortOrder
        //    };
        //    return View(vm);
        //}


        public IActionResult Create()
        {
            return View(new Note());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Note note)
        {
            
            note.CreateDate = DateTime.Now;
            note.updatedDate = DateTime.Now;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            note.UserId = userId;

            var match = Regex.Match(note.Content, @"#(\w+)");
            if (match.Success)
            {
                note.Tag = match.Groups[1].Value;
            }

            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync();

            TempData["Message"] = "Not başarıyla eklendi";
            TempData["MessageType"] = "success";
            return RedirectToAction("Index");

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(Note note)
        {
            var existedValue = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == note.Id);
            if (existedValue != null)
            {
                existedValue.Title = note.Title;
                existedValue.Content = note.Content;
                var match = Regex.Match(note.Content, @"#(\w+)");
                if (match.Success)
                {
                    existedValue.Tag = match.Groups[1].Value;
                }
                existedValue.updatedDate = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var existedValue = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            if (existedValue == null)
            {
                TempData["Message"] = "Böyle bir değer bulunamadı";
                TempData["MessageType"] = "info";
            }
            return View(existedValue);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var existedValue = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            if (existedValue == null)
            {
                TempData["Message"] = "Böyle bir değer bulunamadı";
                TempData["MessageType"] = "info";
            }
            _dbContext.Notes.Remove(existedValue);
            await _dbContext.SaveChangesAsync();
            TempData["Message"] = "Not başarıyla silindi";
            TempData["MessageType"] = "success";
            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult FilterByTag(string tag)
        {
            var filteredNotes = _dbContext.Notes
                .Where(n => n.Tag == tag)
                .ToList();

            return View("Index", filteredNotes);

        }
        
        public async Task<IActionResult> DeleteTag(int id)
        {
            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            if(note != null) note.Tag = null;
            TempData["Message"] = "Tag Silindi!";
            TempData["MessageType"] = "error";
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
       
        public async Task<IActionResult> AddTag(int id,string tag)
        {
            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            if (note != null) note.Tag = tag;
            TempData["Message"] = "Tag eklendi!";
            TempData["MessageType"] = "success";
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Share(int id)
        {
            return null;
        }

        

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
