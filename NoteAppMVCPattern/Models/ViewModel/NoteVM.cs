namespace NoteAppMVCPattern.Models.ViewModel
{
    public class NoteVM
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int NotebookId { get; set; } 
        public AppUserVM User { get; set; } 
    }
}
