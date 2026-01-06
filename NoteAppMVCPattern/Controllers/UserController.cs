using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NoteApp.WebUI.Models;
using NoteApp.WebUI.Models.ViewModel;
using NoteApp.Application.Services.Notebooks;
using System;
using System.Security.Claims;
using static NoteApp.Core.Entities.AppUser;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NoteApp.Core.Entities;
using NoteApp.Infrastructure.Models;

namespace NoteApp.WebUI.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDBContext _dbContext;
        private readonly INotebookService _notebookService;
        public UserController(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,AppDBContext context,INotebookService notebookService)
        {
            _dbContext = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _notebookService = notebookService;            
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        // POST: /Account/Register
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {                    
                    var Id = await _notebookService.EnsureNotebook(user.Id); // Geriye notebook Id dönüyor.
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Note", new {notebookId = Id} );
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == model.UserName);
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName, 
                    model.Password, 
                    model.RememberMe, lockoutOnFailure:false
                    );

                if (result.Succeeded)
                {                    
                    var Id = await _notebookService.EnsureNotebook(user.Id);
                    var NotebookList = await _notebookService.ListAll(user.Id);
                    if (NotebookList.Count == 1)
                    return RedirectToAction("Index", "Note", new {notebookId = Id });
                    else return RedirectToAction("Index", "Notebook");
                }
               
                ModelState.AddModelError("", "Username or password is invalid");              
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","User");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(AppUser user)
        {
            var existedValue = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (existedValue != null)
            {
                existedValue.UserName = user.UserName;
                await _dbContext.SaveChangesAsync();

            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(string id)
        {
            var existedValue = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (existedValue == null)
            {
                TempData["Message"] = "There is no such user";
            }
            return View(existedValue);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var existedValue = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (existedValue == null)
            {
                TempData["Message"] = "There is no such user";
            }
            _dbContext.Users.Remove(existedValue);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index","User");
        }
    }

    //public class UserController : Controller
    //{
    //    private readonly UserManager<AppUser> _userManager;
    //    private readonly AppDBContext _dbContext;

    //    public UserController(UserManager<AppUser> userManager, AppDBContext dbContext)
    //    {
    //        _userManager = userManager;     // ⇒ Identity işlemleri için
    //        _dbContext = dbContext;         // ⇒ Note gibi diğer DbSet işlemleri için
    //    }
    //    public IActionResult Index()
    //    {
    //        return View(_userManager.Users.ToList());
    //    }


    

    //    [HttpPost]
    //    public IActionResult Edit(AppUser user)
    //    {
    //        var existing = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
    //        if (existing == null) return NotFound();

    //        existing.UserName = user.UserName;
    //        _dbContext.SaveChanges();
    //        return RedirectToAction("Index");
    //    }

    //    [HttpPost] 
    //    public IActionResult Delete(string id)
    //    {
    //        //return Content($"delete başarılı {id}"); // değiştir

    //        var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
    //        if (user != null)
    //        {
    //            _dbContext.Users.Remove(user);
    //            _dbContext.SaveChanges();
    //            TempData["Message"] = $"{user.UserName} silindi.";
    //        }
    //        else
    //        {
    //            TempData["Message"] = "Kullanıcı bulunamadı.";
    //        }

    //        return RedirectToAction("Index");
    //    }


    //    public IActionResult Details(string id)
    //    {
    //        var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);
    //        if (user == null) return NotFound();

    //        return View(user);
    //    }
    //}
}
