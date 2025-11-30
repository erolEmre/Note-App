using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NoteApp.WebUI.Models;
using NoteApp.WebUI.Models.ViewModel;
using NoteApp.Application.Services;
using NoteApp.Application.Services.Notebooks;
using NoteApp.Application.Services.Notes;
using System.Security.Claims;
using NoteApp.Core.Entities;
using static Azure.Core.HttpHeader;

namespace NoteApp.WebUI.Controllers
{
    public class NotebookController : BaseController
    {
        private readonly INotebookService _notebookService;
        private readonly INoteService _noteService;
        public NotebookController(INotebookService notebookService,INoteService noteService) 
        { 
            _noteService = noteService;
            _notebookService = notebookService;
        }
        public async Task<IActionResult> Index()
        {
            
            var Notebooks = await _notebookService.ListAll(UserId);
            var Notes = Notebooks.SelectMany(x=> x.Notes ?? new List<Note>()).ToList();
            NotebookVM notebook = new NotebookVM
            {
                notebooks = Notebooks,
                Notes = Notes
            };
           
            return View(notebook);
        }
        

        [HttpGet]
        public IActionResult NotebookDetails(int id)
        {
            var mynotebook = _notebookService.Get(id);
            // Index,Note'daki isimde "notebookId" olmalı
            return RedirectToAction("Index", "Note", new { notebookId = id });
        }
        
       
        public IActionResult Create()
        {
            
            return View(new Notebook());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notebook notebook)
        {
            notebook.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //notebook.User = User.

            await _notebookService.Add(notebook);
            return RedirectToAction("Index","Note", new {notebookId = notebook.Id});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var notebook = await _notebookService.Get(id);
            await _notebookService.Delete(notebook);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var notebook = await _notebookService.Get(id);
            if(notebook != null)
            return View(notebook);
            else return BadRequest();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Notebook notebook)
        {
            if(notebook != null)
            await _notebookService.Update(notebook);
            return RedirectToAction("Index");
        }
        
    }
}
