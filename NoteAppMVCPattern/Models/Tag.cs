namespace NoteAppMVCPattern.Models
{
    public class Tag
    {
         public int Id { get; set; }
         public string TagName { get; set; }        
         public string TagColor { get; set; }

         public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
