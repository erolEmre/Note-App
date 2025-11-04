namespace NoteAppMVCPattern.Models.ViewModel
{
    public class NotebookVM
    {
        public List<Notebook> notebooks { get; set; } = new();
        public List<Note> Notes { get; set; }
    }
}
