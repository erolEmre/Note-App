using System.ComponentModel.DataAnnotations;

namespace NoteApp.Core.Entities
{
    public class Notebook
    {
        public int Id { get; set; }
        //[Display(Name = "İsim")]
        public string Name { get; set; }
        //[Display(Name = "Açıklama")]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public string? Color { get; set; }

        // Tag alanı eklenebilir.. Çoka çok ilişki
        public ICollection<Note> Notes { get; set; }
        public string? UserId { get; set; } // foreign key
        public virtual AppUser? User { get; set; } // navigation property

    }
}
