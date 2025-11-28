using NoteApp.Core.Entities;
namespace NoteApp.WebUI.Models.ViewModel
{
    public class NotebookVM
    {
        public List<Notebook> notebooks { get; set; } = new();
        public List<Note> Notes { get; set; }
    }
}
