using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NoteApp.WebUI.Controllers
{
    public abstract class BaseController : Controller
    {      
            protected int? NotebookId => HttpContext.Session.GetInt32("NotebookId");
            protected string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
