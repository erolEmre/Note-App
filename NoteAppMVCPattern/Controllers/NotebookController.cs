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
            var notebook = await _notebookService.Get(1);

            var NotebookVM = new NotebookVM();
            NotebookVM.notebooks.Add(notebook);
            

            return View(NotebookVM);
        }

        [HttpGet]
        public IActionResult NotebookDetails(int id)
        {
            var mynotebook = _notebookService.Get(id);
            return View(mynotebook);
        }
    }
}
