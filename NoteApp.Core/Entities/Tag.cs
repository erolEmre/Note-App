namespace NoteApp.Core.Entities
{
    public class Tag
    {
         public int Id { get; set; }
         public string TagName { get; set; }        
         public string TagColor { get; set; }
         public int TagUsageCount { get; set; } = 0;

         public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
