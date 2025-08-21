using System.ComponentModel.DataAnnotations;

namespace NoteAppMVCPattern.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }

        public string? UserId { get; set; } // foreign key
        public virtual AppUser? User { get; set; } // navigation property
    }
}
