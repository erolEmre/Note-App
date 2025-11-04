using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Models.ViewModel;
using NoteAppMVCPattern.Services;
using NoteAppMVCPattern.Services.Notebooks;
using NoteAppMVCPattern.Services.Notes;
using System.Security.Claims;
using static Azure.Core.HttpHeader;

namespace NoteAppMVCPattern.Controllers
{
    public class NotebookController : Controller
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Notebooks = await _notebookService.ListAll(userId);
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
        
    }
}
