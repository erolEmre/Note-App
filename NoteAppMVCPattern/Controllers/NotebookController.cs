using Microsoft.AspNetCore.Mvc;

namespace NoteAppMVCPattern.Controllers
{
    public class NotebookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
