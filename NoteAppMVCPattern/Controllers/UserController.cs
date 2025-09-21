﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Models.ViewModel;
using System;
using System.Security.Claims;
using static NoteAppMVCPattern.Models.ViewModel.AppUserVM;

namespace NoteAppMVCPattern.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDBContext _dbContext;
        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,AppDBContext context)
        {
            _dbContext = context;
            _userManager = userManager;
            _signInManager = signInManager;
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
                    TempData["Message"] = "Kayıt başarılı.";
                    TempData["MessageType"] = "success"; 
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Note");
                }

                foreach (var error in result.Errors)
                {
                    TempData["Message"] = error.Description;
                    TempData["MessageType"] = "error";
                }
            }

            return RedirectToAction("Index");
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
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName, 
                    model.Password, 
                    model.RememberMe, lockoutOnFailure:false
                    );

                if (result.Succeeded)
                {                  
                    TempData["Message"] = "Giriş Başarılı";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Index", "Note");
                }

                ModelState.AddModelError("", "Geçersiz giriş bilgileri.");              
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
                TempData["Message"] = "Böyle bir değer bulunamadı";
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
                TempData["Message"] = "Böyle bir değer bulunamadı";
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
