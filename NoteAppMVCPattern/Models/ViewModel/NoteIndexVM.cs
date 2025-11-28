using NoteApp.Core.Entities;
namespace NoteApp.WebUI.Models.ViewModel
{
    public class NoteIndexVM
    {
        public List<Note> Notes { get; set; }
        public List<Tag> Tags { get; set; }

        public List<int> SelectedTagIds { get; set; }
        public string ViewMode { get; set; }
        public string SortOrder { get; set; }
        public Tag CurrentTag { get; set; }
        public List<Notebook> Notebooks { get; set; }
        public int NotebookID { get; set; }
    }

}
