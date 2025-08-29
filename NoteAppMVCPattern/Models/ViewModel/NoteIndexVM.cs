namespace NoteAppMVCPattern.Models.ViewModel
{
    public class NoteIndexVM
    {
        public List<Note> Notes { get; set; } = new List<Note>();
        public List<string?> Tags { get; set; } = new();
        public string ViewMode { get; set; } = "grid";
        public string? CurrentTag { get; set; }
        public string? SortOrder { get; set; }
    }

}
