using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Models.ViewModel;
using NoteAppMVCPattern.Services;
using NoteAppMVCPattern.Services.Notebooks;

namespace NoteAppMVCPattern.Controllers
{
    public class NotebookController : Controller
    {
        private readonly INotebookService _notebookService;
        public NotebookController(INotebookService notebookService) 
        { 
            _notebookService = notebookService;
        }
        public async Task<IActionResult> Index()
        {
            var notebook = await _notebookService.ListAll();

            var NotebookVM = new NotebookVM();
            foreach (var item in notebook)
            {
            NotebookVM.notebooks.Add(item);
            }
            
            return View(NotebookVM);
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
            await _notebookService.Add(notebook);
            return RedirectToAction("Index");
        }

    }
}
