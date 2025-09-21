namespace NoteAppMVCPattern.Models.ViewModel
{
    public class ViewModeVM
    {        
            public List<Note> Notes { get; set; }
            public string ViewMode { get; set; } // "grid", "list", "compact"

    }
}
