using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Migrations;
using NoteAppMVCPattern.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;


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
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notes = await _dbContext.Notes
                .Where(n => n.UserId == userId)
                .Include(n => n.User)
                .ToListAsync();
            // Son düzenlemeye göre notları sırala
            return View(notes);

        }

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
            note.Tag = "#$inan Engin";

            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync();

            TempData["msg"] = "Not başarıyla eklendi";
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
                note.updatedDate = DateTime.Now;
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
                TempData["msg"] = "Böyle bir değer bulunamadı";
            }
            return View(existedValue);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var existedValue = await _dbContext.Notes.FirstOrDefaultAsync(x => x.Id == id);
            if (existedValue == null)
            {
                TempData["msg"] = "Böyle bir değer bulunamadı";
            }
            _dbContext.Notes.Remove(existedValue);
            await _dbContext.SaveChangesAsync();
            TempData["msg"] = "Not başarıyla silindi";
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
